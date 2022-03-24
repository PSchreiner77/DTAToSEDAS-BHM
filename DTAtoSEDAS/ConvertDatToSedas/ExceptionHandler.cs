using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDatToSedas
{

    internal class ExceptionHandler
    {
        delegate void Action MyDelegate();


        public void Messager()
        {
            ExceptionMessageHandler += () => Message.Show("test");
        }
    }
}
