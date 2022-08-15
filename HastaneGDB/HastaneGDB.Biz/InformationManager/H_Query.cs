using HastaneGDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HastaneGDB.Biz.InformationManager
{

public class H_Query
    {
    HASTANEPROJEEntities gdb;
    public H_Query()
    {
        gdb = new HASTANEPROJEEntities();  
    }

    public List<IZIN> G_Izin_Bittimi(DateTime bugun)
    {
        return (from i in gdb.IZIN where i.BITISTARIH > bugun select i).ToList();
    }

    public List<GUNLUKIZIN> G_Izinli_DrVarmi(DateTime now)
    {
        return (from i in gdb.GUNLUKIZIN where i.TARIH == now select i).ToList();
    }

    public List<HASTA> Hasta_Gecmisin(string tc)
    {
        return (from i in gdb.HASTA where i.TCNO == tc select i).ToList();

    }
    public List<HASTA> IsRandevu(string tc)
    {
        return (from i in gdb.HASTA where i.TCNO == tc && i.RANDEVUMU.Value select i).ToList(); 
    }

    public List<string> GetPoliklinik()
    {

        return (from i in gdb.DOKTORLAR where i.DR_ID!=0 select i.POLIKLINIK).Distinct().ToList();
    }

    public List<string> GetDoctor(string poli)
    {

        return (from i in gdb.DOKTORLAR where i.POLIKLINIK == poli && i.IZINLIMI==false select i.AD_SOYAD).Distinct().ToList();
    }

    public List<HASTA> HastaTakip(DateTime bugun)
    {
      
        return (from i in gdb.HASTA where i.TARIH==bugun select i).ToList();
    }

    public List<HASTA> KontrolGunu(string tcno)
    {

        return (from i in gdb.HASTA where i.TCNO == tcno && i.BAKILDIMI==true  select i).ToList();

    }
    }
}
