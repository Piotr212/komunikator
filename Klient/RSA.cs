using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klient
{
    public class RSA
    {
        List<int> lista_liczb_pierwszych = new List<int>();
        Random los = new Random();
       public Dictionary<string,int> generowanie_klucza()
        {
            Dictionary<string, int> klucze = new Dictionary<string, int>();
            liczby_pierwsze(20);
            int p = losowa_liczba_pierwsza();
            int q = losowa_liczba_pierwsza();
            int fi = (p - 1) * (q - 1);
            int N = p * q;
            liczby_pierwsze(fi);
            int e = losowa_liczba_pierwsza();
            int d = modular_multiplicative_inverse(e, fi);
            klucze.Add("N", N);
            klucze.Add("e", e);
            klucze.Add("d", d);
            return klucze;
        }
        public byte [] szyfrowanie(int e,int N,string wiadomosc)
        {
            char[] znaki = wiadomosc.ToCharArray();
            string zaszyfrowane="";
            for (int i = 0; i < znaki.Length; i++)
            {
                int kod =znaki[i];
                double a = Convert.ToDouble(kod);
                double b = Convert.ToDouble(e);
                double m = (Math.Pow(a, b))%Convert.ToDouble(N);
                zaszyfrowane += m+ ":";
            }
            byte[] bity = new byte[1500];
            ASCIIEncoding enc = new ASCIIEncoding();
            bity = enc.GetBytes(zaszyfrowane);
            return bity;
        }
         public string odszyfrowanie(string otrzymany,int d,int N)
        {
            string[] znaki = otrzymany.Split(':');
            string wiadomosc = "";
            for (int i = 0; i < znaki.Length-1; i++)
            {
                int liczba = Int32.Parse(znaki[i]);
                int m =(int)( (Math.Pow(Convert.ToDouble(liczba), Convert.ToDouble(d))) % Convert.ToDouble(N));
                char znak = (char)m;
                wiadomosc += znak;
            }
            
            return wiadomosc;
        }
        void liczby_pierwsze(int n)
        {
            lista_liczb_pierwszych.Clear();
            bool[] czy_pierwsza = new bool[n + 1];
            for (int i = 2; i <= n; i++)
            {
                czy_pierwsza[i] = true;
            }
            for (int i = 2; i <= n; i++)
            {
                if (czy_pierwsza[i])
                {
                    for (int j = i * 2; j <= n; j += i)
                    {
                        czy_pierwsza[j] = false;
                    }
                }
            }
            for (int i = 2; i <= n; i++)
            {
                if (czy_pierwsza[i])
                {
                    lista_liczb_pierwszych.Add(i);
                }
            }
        }
        int losowa_liczba_pierwsza()
        {
            int liczba = lista_liczb_pierwszych[los.Next(0, lista_liczb_pierwszych.Count)];

            return liczba;
        }
        int modular_multiplicative_inverse(int a, int n)//kod z wikipedi
        {
            int t = 0;
            int nt = 1;
            int r = n;
            if (n < 0)
            {
                n = -n;
            }
            if (a < 0)
            {
                a = n - (-a % n);
            }
            int nr = a % n;
            while (nr != 0)
            {
                int quot = (r / nr) | 0;
                int tmp = nt; nt = t - quot * nt; t = tmp;
                tmp = nr; nr = r - quot * nr; r = tmp;
            }
            if (r > 1) { return -1; }
            if (t < 0) { t += n; }
            return t;
        }
    }
}
