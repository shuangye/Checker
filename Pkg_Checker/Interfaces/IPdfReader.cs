using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pkg_Checker.Interfaces
{
    public interface IPdfReader
    {
        void Read(String pdfPath);
        
        void CheckCommonFields();
        void CheckWorkProductType();
        void CheckCheckList();

        List<String> GetDefects();
    }
}
