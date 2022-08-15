using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HastaneGDB.Data;

namespace HastaneGDB.Biz.InputManager
{
   
 public   class Query
    {
       HASTANEPROJEEntities gdb;
   

        public Query()
        {
            gdb = new HASTANEPROJEEntities();
        }

        public int Control(string tc, string sifre)
        {
            var control = (from i in gdb.GIRIS where i.TCNO == tc && i.SIFRE == sifre select i.ID).SingleOrDefault();
          if (control == null)
              return 0;
          else
              return control.Value;
        }


    }
}
