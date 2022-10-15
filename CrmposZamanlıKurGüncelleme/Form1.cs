using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CrmposZamanlıKurGüncelleme
{
    public partial class Form1 : Form
    {
        MRTREntities db = new MRTREntities();
        public Form1()
        {
            InitializeComponent();
        }

        DataTable dt = new DataTable();        
        DataRow dr;
        public DateTime ActualCurrencyDate { get; set; }
        string url;
        XmlTextReader rdr;
        XmlDocument XmlDoc;

        

        private void GenerateApiUrl()
        {
            url = string.Empty;
                       
            url = "http://www.tcmb.gov.tr/kurlar/today.xml";
           
            rdr = new XmlTextReader(url);
        }

        public DataTable source()
        {
            dt.Rows.Clear();
            dt.Columns.Clear();
            try
            {
                dt.Columns.Add(new DataColumn("Sıra", typeof(int)));
                dt.Columns.Add(new DataColumn("Tarih", typeof(DateTime)));
                dt.Columns.Add(new DataColumn("Kod", typeof(string)));
                dt.Columns.Add(new DataColumn("Döviz_alış", typeof(decimal)));
                dt.Columns.Add(new DataColumn("Döviz_satış", typeof(decimal)));

                

                for (int attempts = 0; attempts <= 5; attempts++)
                {
                    try
                    {
                        GenerateApiUrl();
                        XmlDoc = new XmlDocument();
                        XmlDoc.Load(rdr);

                        break;
                    }
                    catch (WebException ex)
                    {
                        if (ex.Response != null)
                        {
                            // 404 not found
                            HttpWebResponse errorResponse = ex.Response as HttpWebResponse;
                            if (errorResponse.StatusCode == HttpStatusCode.NotFound)
                            {
                                // bir gün öncesi kontrol edilir
                                ActualCurrencyDate = ActualCurrencyDate.AddDays(-1);
                            }
                            else
                            {
                                throw new Exception("Kur bilgisi bulunamadı.");
                            }
                        }
                        else
                        {
                            throw new Exception("Kur bilgisi bulunamadı.");
                        }
                    }
                }

                // Load metodu ile xml yüklüyoruz
                XmlNode tarih = XmlDoc.SelectSingleNode("/Tarih_Date/@Tarih");
                XmlNodeList mylist = XmlDoc.SelectNodes("/Tarih_Date/Currency");
                XmlNodeList adi = XmlDoc.SelectNodes("/Tarih_Date/Currency/Isim");
                XmlNodeList kod = XmlDoc.SelectNodes("/Tarih_Date/Currency/@Kod");
                XmlNodeList doviz_alis = XmlDoc.SelectNodes("/Tarih_Date/Currency/ForexBuying");
                XmlNodeList doviz_satis = XmlDoc.SelectNodes("/Tarih_Date/Currency/ForexSelling");
                XmlNodeList efektif_alis = XmlDoc.SelectNodes("/Tarih_Date/Currency/BanknoteBuying");
                XmlNodeList efektif_satis = XmlDoc.SelectNodes("/Tarih_Date/Currency/BanknoteSelling");

                
                int x = 17;
               
                
                for (int i = 0; i < x; i++)
                {
                    dr = dt.NewRow();
                    dr[0] = (i + 1);
                    dr[1] = DateTime.Now.Date; //.ToString("dd.MM.yyyy");
                    
                    dr[2] = kod.Item(i).InnerText.ToString();
                    // Kod satırları
                    dr[3] = decimal.Parse(doviz_alis.Item(i).InnerText, CultureInfo.InvariantCulture);
                    // Döviz Alış
                    dr[4] = decimal.Parse(doviz_satis.Item(i).InnerText, CultureInfo.InvariantCulture);
                    // Döviz  Satış
                    //dr[5] = efektif_alis.Item(i).InnerText.ToString();
                    //// Efektif Alış
                    //dr[6] = efektif_satis.Item(i).InnerText.ToString();
                    // Efektif Satış.
                    dt.Rows.Add(dr);
                }
            }
            catch (Exception expc)
            {
                MessageBox.Show(expc.ToString(), "İnfo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
                     
            return dt;
            // DataTable ı döndürüyoruz.
        }
        int dvzid;

        private void timer1_Tick(object sender, EventArgs e)
        {
            string saat, dakika,saat2,dakika2,saniye;
            saat = textBox1.Text;
            dakika = textBox2.Text;
            saat2 = textBox3.Text;
            dakika2 = textBox4.Text;
            saniye = "1";


            if ((Convert.ToString(DateTime.Now.Hour)) == saat && (Convert.ToString(DateTime.Now.Minute) == dakika) && (Convert.ToString(DateTime.Now.Second)) == saniye)
            {
                sync();
            }

            if ((Convert.ToString(DateTime.Now.Hour)) == saat2 && (Convert.ToString(DateTime.Now.Minute) == dakika2) && (Convert.ToString(DateTime.Now.Second)) == saniye)
            {
                sync();
            }
        }

        private void sync()
        {
            dataGridView1.DataSource = source();
            CultureInfo culture = new CultureInfo("en-US", true);

            // en-US kültüründe ondalık ayracı zaten "." olduğu için değerlendirmeye gerek yok.          


            for (int i = 0; i < source().Rows.Count; i++)
            {
                DataRow dr = source().Rows[i];
                switch (dr["Kod"].ToString())
                {
                    case "USD":
                        dvzid = 2;
                        break;
                    case "EUR":
                        dvzid = 3;
                        break;
                    case "AUD":
                        dvzid = 4;
                        break;
                    case "DKK":
                        dvzid = 5;
                        break;
                    case "GBP":
                        dvzid = 6;
                        break;
                    case "CHF":
                        dvzid = 7;
                        break;
                    case "SEK":
                        dvzid = 8;
                        break;
                    case "CAD":
                        dvzid = 9;
                        break;
                    case "KWD":
                        dvzid = 10;
                        break;
                    case "NOK":
                        dvzid = 11;
                        break;
                    case "SAR":
                        dvzid = 12;
                        break;
                    case "JPY":
                        dvzid = 13;
                        break;
                    case "BGN":
                        dvzid = 14;
                        break;
                    case "RUB":
                        dvzid = 15;
                        break;
                    case "IRR":
                        dvzid = 16;
                        break;
                    case "CNY":
                        dvzid = 17;
                        break;
                    case "RON":
                        dvzid = 18;
                        break;
                    default:
                        dvzid = 0;
                        break;
                }


                var dcalis = dr.Field<decimal>("Döviz_alış");
                var dcsatis = dr.Field<decimal>("Döviz_satış");
                var tatarih = DateTime.Now.Date;
                int count = db.TCMB_KUR_CRMPOS.Count(a => a.TARIH == tatarih);

                if (count > 0)
                {
                    var x = db.TCMB_KUR_CRMPOS.Where(a => a.DOVIZ_AD == dvzid && a.TARIH == tatarih).First();
                    x.TARIH = tatarih;
                    x.ALIS = dcalis;
                    x.SATIS = dcsatis;
                    x.DOVIZ_AD = dvzid;
                    x.SIRA = dr.Field<int>("Sıra");
                }
                else
                {
                    db.TCMB_KUR_CRMPOS.Add(new TCMB_KUR_CRMPOS()
                    { //add data to class objects variable
                        TARIH = dr.Field<DateTime>("Tarih"),
                        ALIS = dcalis,
                        SATIS = dcsatis,
                        DOVIZ_AD = dvzid,
                        SIRA = dr.Field<int>("Sıra")
                    });
                }


            }
            db.SaveChanges();
            label2.Text = DateTime.Now + "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            button1.Enabled = false;
            timer1.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
