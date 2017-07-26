using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace 共通
{
    public class MultipleControl : IMultipleControl
    {

        private Semaphore semaphore = null;
        bool createdNew = false;
        string name;
        public MultipleControl(Window window)
        {
            this.name = window.GetType().FullName;
            
        }

        public bool IsCreate()
        {
            semaphore = new Semaphore(1, 1, name, out createdNew);
            if (createdNew)
            {
                return true;
            }
            Releace();
            return false;
        }

        public void Releace()
        {
            semaphore.Dispose();
            semaphore = null;
        }
    }

    public interface IMultipleControl
    {
        
        bool IsCreate();
        void Releace();
    }


}
