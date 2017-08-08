'*****************************************************
'
'VB Dll注入模板
'将此文件复制到你的VB项目即可
'
'*****************************************************
Namespace Injecting

    '''<summary>
    ''' Dll注入使用
    ''' </summary>
    Public Class Injector

        '''<summary>
        ''' DllMain
        ''' </summary>
        ''' <param name="arg">无效参数，忽视，但此形参不能删除！！！</param>
        ''' <returns></returns>
        Public Shared Function DllMain(ByVal arg As String) As Integer
            System.Windows.Forms.MessageBox.Show("VBInjector")
            '这句话改成你的一段代码，比如Hook 读写内存什么的
            Return 0
        End Function

    End Class

End Namespace
