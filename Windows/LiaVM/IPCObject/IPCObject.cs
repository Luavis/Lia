using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPCObject
{
    public delegate bool IPCCaller(String str);

    public class IPCRemoteObject : MarshalByRefObject
    {
        static IPCCaller consoleListen;

        public IPCRemoteObject() : base()
        {
            
        }

        public IPCRemoteObject(IPCCaller _consoleListen) : this()
        {
            consoleListen = _consoleListen;
        }

        public bool getConsoleString(String str)
        {
            consoleListen(str);
            return true;
        }
    }
}
