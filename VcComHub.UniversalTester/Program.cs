using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using VcComHub.Memory;
using System.Threading;

internal class Program
{
    static void Main(string[] args)
    {
        Memory.InjectDllToProcess(Process.GetProcessesByName("vietcong").First(), "");        
    }
}
