#include <windows.h>

int main()
{
#ifdef _WIN64
#ifdef _DEBUG
	LoadLibrary(L"C:\\Users\\wwh10\\Documents\\Visual Studio 2017\\Projects\\FastWin32\\x64\\Debug\\DInvoker4.dll");
#else
	LoadLibrary(L"C:\\Users\\wwh10\\Documents\\Visual Studio 2017\\Projects\\FastWin32\\x64\\Release\\DInvoker4.dll");
#endif
#else
#ifdef _DEBUG
	HMODULE hModule = LoadLibrary(L"C:\\Users\\wwh10\\Documents\\Visual Studio 2017\\Projects\\FastWin32\\Debug\\DInvoker4.dll");
#else
	LoadLibrary(L"C:\\Users\\wwh10\\Documents\\Visual Studio 2017\\Projects\\FastWin32\\Release\\DInvoker4.dll");
#endif
#endif
//#ifdef _WIN64
//#ifdef _DEBUG
//	LoadLibrary(L"C:\\Users\\wwh10\\Documents\\Visual Studio 2017\\Projects\\FastWin32\\x64\\Debug\\DInvoker2.dll");
//#else
//	LoadLibrary(L"C:\\Users\\wwh10\\Documents\\Visual Studio 2017\\Projects\\FastWin32\\x64\\Release\\DInvoker2.dll");
//#endif
//#else
//#ifdef _DEBUG
//	LoadLibrary(L"C:\\Users\\wwh10\\Documents\\Visual Studio 2017\\Projects\\FastWin32\\Debug\\DInvoker2.dll");
//#else
//	LoadLibrary(L"C:\\Users\\wwh10\\Documents\\Visual Studio 2017\\Projects\\FastWin32\\Release\\DInvoker2.dll");
//#endif
//#endif
	system("pause");
	return 0;
}
