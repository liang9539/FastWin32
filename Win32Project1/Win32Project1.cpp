// Win32Project1.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include <immintrin.h>

#include "stdafx.h"  


typedef IMAGE_SECTION_HEADER(*PIMAGE_SECTION_HEADERS)[1];

// 计算对齐后的大小   
unsigned long GetAlignedSize(unsigned long Origin, unsigned long Alignment)
{
	return (Origin + Alignment - 1) / Alignment * Alignment;
}

// 计算加载pe并对齐需要占用多少内存   
// 未直接使用OptionalHeader.SizeOfImage作为结果是因为据说有的编译器生成的exe这个值会填0   
unsigned long CalcTotalImageSize(PIMAGE_DOS_HEADER MzH
	, unsigned long FileLen
	, PIMAGE_NT_HEADERS peH
	, PIMAGE_SECTION_HEADERS peSecH)
{
	unsigned long res;
	// 计算pe头的大小   
	res = GetAlignedSize(peH->OptionalHeader.SizeOfHeaders
		, peH->OptionalHeader.SectionAlignment
	);

	// 计算所有节的大小   
	for (int i = 0; i < peH->FileHeader.NumberOfSections; ++i)
	{
		// 超出文件范围   
		if (peSecH[i]->PointerToRawData + peSecH[i]->SizeOfRawData > FileLen)
			return 0;
		else if (peSecH[i]->VirtualAddress)//计算对齐后某节的大小   
		{
			if (peSecH[i]->Misc.VirtualSize)
			{
				res = GetAlignedSize(peSecH[i]->VirtualAddress + peSecH[i]->Misc.VirtualSize
					, peH->OptionalHeader.SectionAlignment
				);
			}
			else
			{
				res = GetAlignedSize(peSecH[i]->VirtualAddress + peSecH[i]->SizeOfRawData
					, peH->OptionalHeader.SectionAlignment
				);
			}
		}
		else if (peSecH[i]->Misc.VirtualSize < peSecH[i]->SizeOfRawData)
		{
			res += GetAlignedSize(peSecH[i]->SizeOfRawData
				, peH->OptionalHeader.SectionAlignment
			);
		}
		else
		{
			res += GetAlignedSize(peSecH[i]->Misc.VirtualSize
				, peH->OptionalHeader.SectionAlignment
			);
		}// if_else   
	}// for   

	return res;
}




// 加载pe到内存并对齐所有节   
BOOL AlignPEToMem(void *Buf
	, long Len
	, PIMAGE_NT_HEADERS &peH
	, PIMAGE_SECTION_HEADERS &peSecH
	, void *&Mem
	, unsigned long &ImageSize)
{
	PIMAGE_DOS_HEADER SrcMz;// DOS头   
	PIMAGE_NT_HEADERS SrcPeH;// PE头   
	PIMAGE_SECTION_HEADERS SrcPeSecH;// 节表   

	SrcMz = (PIMAGE_DOS_HEADER)Buf;
	_rdrand32_step
	if (Len < sizeof(IMAGE_DOS_HEADER))
		return FALSE;

	if (SrcMz->e_magic != IMAGE_DOS_SIGNATURE)
		return FALSE;

	if (Len < SrcMz->e_lfanew + (long)sizeof(IMAGE_NT_HEADERS))
		return FALSE;

	SrcPeH = (PIMAGE_NT_HEADERS)((int)SrcMz + SrcMz->e_lfanew);
	if (SrcPeH->Signature != IMAGE_NT_SIGNATURE)
		return FALSE;

	if ((SrcPeH->FileHeader.Characteristics & IMAGE_FILE_DLL) ||
		(SrcPeH->FileHeader.Characteristics & IMAGE_FILE_EXECUTABLE_IMAGE == 0) ||
		(SrcPeH->FileHeader.SizeOfOptionalHeader != sizeof(IMAGE_OPTIONAL_HEADER)))
	{
		return FALSE;
	}


	SrcPeSecH = (PIMAGE_SECTION_HEADERS)((int)SrcPeH + sizeof(IMAGE_NT_HEADERS));
	ImageSize = CalcTotalImageSize(SrcMz, Len, SrcPeH, SrcPeSecH);

	if (ImageSize == 0)
		return FALSE;

	Mem = VirtualAlloc(NULL, ImageSize, MEM_COMMIT, PAGE_EXECUTE_READWRITE); // 分配内存   
	if (Mem != NULL)
	{
		// 计算需要复制的PE头字节数   
		unsigned long l = SrcPeH->OptionalHeader.SizeOfHeaders;
		for (int i = 0; i < SrcPeH->FileHeader.NumberOfSections; ++i)
		{
			if ((SrcPeSecH[i]->PointerToRawData) &&
				(SrcPeSecH[i]->PointerToRawData < l))
			{
				l = SrcPeSecH[i]->PointerToRawData;
			}
		}
		memmove(Mem, SrcMz, l);
		peH = (PIMAGE_NT_HEADERS)((int)Mem + ((PIMAGE_DOS_HEADER)Mem)->e_lfanew);
		peSecH = (PIMAGE_SECTION_HEADERS)((int)peH + sizeof(IMAGE_NT_HEADERS));

		void *Pt = (void *)((unsigned long)Mem
			+ GetAlignedSize(peH->OptionalHeader.SizeOfHeaders
				, peH->OptionalHeader.SectionAlignment)
			);

		for (int i = 0; i < peH->FileHeader.NumberOfSections; ++i)
		{
			// 定位该节在内存中的位置   
			if (peSecH[i]->VirtualAddress)
				Pt = (void *)((unsigned long)Mem + peSecH[i]->VirtualAddress);

			if (peSecH[i]->SizeOfRawData)
			{
				// 复制数据到内存   
				memmove(Pt, (const void *)((unsigned long)(SrcMz)+peSecH[i]->PointerToRawData), peSecH[i]->SizeOfRawData);
				if (peSecH[i]->Misc.VirtualSize < peSecH[i]->SizeOfRawData)
					Pt = (void *)((unsigned long)Pt + GetAlignedSize(peSecH[i]->SizeOfRawData, peH->OptionalHeader.SectionAlignment));
				else // pt 定位到下一节开始位置   
					Pt = (void *)((unsigned long)Pt + GetAlignedSize(peSecH[i]->Misc.VirtualSize, peH->OptionalHeader.SectionAlignment));
			}
			else
			{
				Pt = (void *)((unsigned long)Pt + GetAlignedSize(peSecH[i]->Misc.VirtualSize, peH->OptionalHeader.SectionAlignment));
			}
		}
	}
	return TRUE;
}



typedef void *(__stdcall *pfVirtualAllocEx)(unsigned long, void *, unsigned long, unsigned long, unsigned long);
pfVirtualAllocEx MyVirtualAllocEx = NULL;

BOOL IsNT()
{
	return MyVirtualAllocEx != NULL;
}

// 生成外壳程序命令行   
char *PrepareShellExe(char *CmdParam, unsigned long BaseAddr, unsigned long ImageSize)
{
	if (IsNT())
	{
		
		wchar_t *Buf = new wchar_t[256];
		memset(Buf, 0, 256);
		GetModuleFileName(0, Buf, 256);
		strcat(Buf, CmdParam);
		return Buf; // 请记得释放内存;-)   
	}
	else
	{
		// Win98下的处理请参考原文;-)   
		// http://community.csdn.net/Expert/topic/4416/4416252.xml?temp=8.709133E-03   
		return NULL;
	}
}

// 是否包含可重定向列表   
BOOL HasRelocationTable(PIMAGE_NT_HEADERS peH)
{
	return (peH->OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_BASERELOC].VirtualAddress)
		&& (peH->OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_BASERELOC].Size);
}




#pragma pack(push, 1)   
typedef struct
{
	unsigned long VirtualAddress;
	unsigned long SizeOfBlock;
} *PImageBaseRelocation;
#pragma pack(pop)   

// 重定向PE用到的地址   
void DoRelocation(PIMAGE_NT_HEADERS peH, void *OldBase, void *NewBase)
{
	unsigned long Delta = (unsigned long)NewBase - peH->OptionalHeader.ImageBase;
	PImageBaseRelocation p = (PImageBaseRelocation)((unsigned long)OldBase
		+ peH->OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_BASERELOC].VirtualAddress);
	while (p->VirtualAddress + p->SizeOfBlock)
	{
		unsigned short *pw = (unsigned short *)((int)p + sizeof(*p));
		for (unsigned int i = 1; i <= (p->SizeOfBlock - sizeof(*p)) / 2; ++i)
		{
			if ((*pw) & 0xF000 == 0x3000)
			{
				unsigned long *t = (unsigned long *)((unsigned long)(OldBase)+p->VirtualAddress + ((*pw) & 0x0FFF));
				*t += Delta;
			}
			++pw;
		}
		p = (PImageBaseRelocation)pw;
	}
}

// 卸载原外壳占用内存   
BOOL UnloadShell(HANDLE ProcHnd, unsigned long BaseAddr)
{
	typedef unsigned long(__stdcall *pfZwUnmapViewOfSection)(unsigned long, unsigned long);
	pfZwUnmapViewOfSection ZwUnmapViewOfSection = NULL;
	BOOL res = FALSE;
	HMODULE m = LoadLibrary(L"ntdll.dll");
	if (m)
	{
		ZwUnmapViewOfSection = (pfZwUnmapViewOfSection)GetProcAddress(m, "ZwUnmapViewOfSection");
		if (ZwUnmapViewOfSection)
			res = (ZwUnmapViewOfSection((unsigned long)ProcHnd, BaseAddr) == 0);
		FreeLibrary(m);
	}
	return res;
}

// 创建外壳进程并获取其基址、大小和当前运行状态   
BOOL CreateChild(char *Cmd, CONTEXT &Ctx, HANDLE &ProcHnd, HANDLE &ThrdHnd,
	unsigned long &ProcId, unsigned long &BaseAddr, unsigned long &ImageSize)
{
	STARTUPINFOA si;
	PROCESS_INFORMATION pi;
	unsigned long old;
	MEMORY_BASIC_INFORMATION MemInfo;
	memset(&si, 0, sizeof(si));
	memset(&pi, 0, sizeof(pi));
	si.cb = sizeof(si);

	BOOL res = CreateProcess(NULL, Cmd, NULL, NULL, FALSE, CREATE_SUSPENDED, NULL, NULL, &si, &pi); // 以挂起方式运行进程;   
	if (res)
	{
		ProcHnd = pi.hProcess;
		ThrdHnd = pi.hThread;
		ProcId = pi.dwProcessId;
		// 获取外壳进程运行状态，[ctx.Ebx+8]内存处存的是外壳进程的加载基址，ctx.Eax存放有外壳进程的入口地址   
		Ctx.ContextFlags = CONTEXT_FULL;
		GetThreadContext(ThrdHnd, &Ctx);
		ReadProcessMemory(ProcHnd, (void *)(Ctx.Ebx + 8), &BaseAddr, sizeof(unsigned long), &old); // 读取加载基址   
		void *p = (void *)BaseAddr;
		// 计算外壳进程占有的内存   
		while (VirtualQueryEx(ProcHnd, p, &MemInfo, sizeof(MemInfo)))
		{
			if (MemInfo.State = MEM_FREE) break;
			p = (void *)((unsigned long)p + MemInfo.RegionSize);
		}
		ImageSize = (unsigned long)p - (unsigned long)BaseAddr;
	}
	return res;
}

// 创建外壳进程并用目标进程替换它然后执行   
HANDLE AttachPE(char *CmdParam, PIMAGE_NT_HEADERS peH, PIMAGE_SECTION_HEADERS peSecH,
	void *Ptr, unsigned long ImageSize, unsigned long &ProcId)
{
	HANDLE res = INVALID_HANDLE_VALUE;
	CONTEXT Ctx;
	HANDLE Thrd;
	unsigned long Addr, Size;
	char *s = PrepareShellExe(CmdParam, peH->OptionalHeader.ImageBase, ImageSize);
	if (s == NULL) return res;
	if (CreateChild(s, Ctx, res, Thrd, ProcId, Addr, Size))
	{
		void *p = NULL;
		unsigned long old;
		if ((peH->OptionalHeader.ImageBase == Addr) && (Size >= ImageSize))
		{// 外壳进程可以容纳目标进程并且加载地址一致   
			p = (void *)Addr;
			VirtualProtectEx(res, p, Size, PAGE_EXECUTE_READWRITE, &old);
		}
		else if (IsNT())
		{
			if (UnloadShell(res, Addr))
			{// 卸载外壳进程占有内存   
				p = MyVirtualAllocEx((unsigned long)res, (void *)peH->OptionalHeader.ImageBase, ImageSize, MEM_RESERVE | MEM_COMMIT, PAGE_EXECUTE_READWRITE);
			}
			if ((p == NULL) && HasRelocationTable(peH))
			{// 分配内存失败并且目标进程支持重定向   
				p = MyVirtualAllocEx((unsigned long)res, NULL, ImageSize, MEM_RESERVE | MEM_COMMIT, PAGE_EXECUTE_READWRITE);
				if (p) DoRelocation(peH, Ptr, p); // 重定向   
			}
		}
		if (p)
		{
			WriteProcessMemory(res, (void *)(Ctx.Ebx + 8), &p, sizeof(DWORD), &old); // 重置目标进程运行环境中的基址   
			peH->OptionalHeader.ImageBase = (unsigned long)p;
			if (WriteProcessMemory(res, p, Ptr, ImageSize, &old))
			{// 复制PE数据到目标进程   
				Ctx.ContextFlags = CONTEXT_FULL;
				if ((unsigned long)p == Addr)
					Ctx.Eax = peH->OptionalHeader.ImageBase + peH->OptionalHeader.AddressOfEntryPoint; // 重置运行环境中的入口地址   
				else
					Ctx.Eax = (unsigned long)p + peH->OptionalHeader.AddressOfEntryPoint;
				SetThreadContext(Thrd, &Ctx);// 更新运行环境   
				ResumeThread(Thrd);// 执行   
				CloseHandle(Thrd);
			}
			else
			{// 加载失败,杀掉外壳进程   
				TerminateProcess(res, 0);
				CloseHandle(Thrd);
				CloseHandle(res);
				res = INVALID_HANDLE_VALUE;
			}
		}
		else
		{// 加载失败,杀掉外壳进程   
			TerminateProcess(res, 0);
			CloseHandle(Thrd);
			CloseHandle(res);
			res = INVALID_HANDLE_VALUE;
		}
	}
	delete[] s;
	return res;
}




/*******************************************************\
{ ******************************************************* }
{ *                 从内存中加载并运行exe               * }
{ ******************************************************* }
{ * 参数：                                                }
{ * Buffer: 内存中的exe地址                               }
{ * Len: 内存中exe占用长度                                }
{ * CmdParam: 命令行参数(不包含exe文件名的剩余命令行参数）}
{ * ProcessId: 返回的进程Id                               }
{ * 返回值： 如果成功则返回进程的Handle(ProcessHandle),   }
{            如果失败则返回INVALID_HANDLE_VALUE           }
{ ******************************************************* }
\*******************************************************/
HANDLE MemExecute(void *ABuffer, long Len, char *CmdParam, unsigned long *ProcessId)
{
	HANDLE res = INVALID_HANDLE_VALUE;
	PIMAGE_NT_HEADERS peH;
	PIMAGE_SECTION_HEADERS peSecH;
	void *Ptr;
	unsigned long peSz;
	if (AlignPEToMem(ABuffer, Len, peH, peSecH, Ptr, peSz))
	{
		res = AttachPE(CmdParam, peH, peSecH, Ptr, peSz, *ProcessId);
		VirtualFree(Ptr, peSz, MEM_DECOMMIT);
	}
	return res;
}

//初始化   
class CInit
{

public:
	CInit()
	{
		MyVirtualAllocEx = (pfVirtualAllocEx)GetProcAddress(GetModuleHandle(L"Kernel32.dll"), "VirtualAllocEx");
	}

}Init;

int APIENTRY WinMain(
	HINSTANCE hInstance,
	HINSTANCE hPrevInstance,
	LPSTR     lpCmdLine,
	int       nCmdShow
)
{
	HANDLE hFile = NULL;
	hFile = ::CreateFile(L"f:\\SourceFromCsdn2.exe"
		, FILE_ALL_ACCESS
		, 0
		, NULL
		, OPEN_EXISTING
		, FILE_ATTRIBUTE_NORMAL
		, NULL
	);
	if (hFile == INVALID_HANDLE_VALUE)
		return -1;

	::SetFilePointer(hFile, 0, NULL, FILE_BEGIN);
	DWORD dwFileSize = ::GetFileSize(hFile, NULL);

	LPBYTE pBuf = new BYTE[dwFileSize];
	memset(pBuf, 0, dwFileSize);

	DWORD dwNumberOfBytesRead = 0;
	::ReadFile(hFile
		, pBuf
		, dwFileSize
		, &dwNumberOfBytesRead
		, NULL
	);

	::CloseHandle(hFile);

	unsigned long ulProcessId = 0;
	MemExecute(pBuf, dwFileSize, "", &ulProcessId);
	delete[] pBuf;


	return 0;
}