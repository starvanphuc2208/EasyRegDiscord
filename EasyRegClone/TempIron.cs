using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronOcr;

namespace easy
{
    sealed class TempIron : IDisposable
    {
        IronTesseract _path;
        public TempIron() 
        { 
            _path = new IronTesseract();
        }

        public IronTesseract Path
        {
            get
            {
                if (_path == null) throw new ObjectDisposedException(GetType().Name);
                return _path;
            }
        }

        ~TempIron() { Dispose(false); }
        public void Dispose() { Dispose(true); }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
            if (_path != null)
            {
                _path = null;
            }
        }

    }
}
