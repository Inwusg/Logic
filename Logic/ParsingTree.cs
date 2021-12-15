using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Text;
using System.Windows.Media.Imaging;

namespace Logic
{
    class ParsingTree
    {
        private int Kol { get; set; }
        private List<string> Out { get; set; }
        public string Formula { get; set; }
        private Dictionary<string, bool> Dictionary { get; set; }
        public List<string> PeremenList { get; set; }
        public ParsingTree(string formula)
        {
            Formula = formula;
            PeremenList = new List<string>();
        }

        public bool Сalculation(Dictionary<string, bool> dictionary, Knot k)
        {
            Dictionary = dictionary;
            return Result(k);
        }
        //(!r=>!(!!(!(!p=>!q)*r)+(p*(!q*!r))))
        //(r+(((p+!q)+!r)*(!p+(q+r))))
        //(((a+b)*c)+(a*(b+c)))
        public string DNF(Knot k)
        {
            RemoveImplication(k);
            Not(k);
            do
            {
                Kol = 0;
                Not(k);
            } while (Kol != 0);
            RemoveNot(k);

            do
            {
                Kol = 0;
                DistributivityDNF(k);
            } while (Kol != 0);

            Out = new List<string>();
            OutFormula(k, 0);
            string str = "";
            foreach (var stroka in Out) str += stroka;
            return str;
        }

        public string KNF(Knot k)
        {
            RemoveImplication(k);
            Not(k);
            do
            {
                Kol = 0;
                Not(k);
            } while (Kol != 0);
            RemoveNot(k);
            do
            {
                Kol = 0;
                DistributivityKNF(k);
            } while (Kol != 0);
            //DistributivityKNF(k);
            Out = new List<string>();
            //return GetString(k, "");
            OutFormula(k, 0);
            string str = "";
            foreach (var stroka in Out) str += stroka;
            return str;

        }

        public int OutFormula(Knot k, int index)
        {
            //   ((a+b)*c)
            if (k.Name == "+" || k.Name == "*" || k.Name == "=>")
            {
                Out.Insert(index, "(");
                index++;
                Out.Insert(index, k.Name);
                Out.Insert(index + 1, ")");
                //return index;
            }
            else
            {
                if (k.Name == "!")
                {
                    Out.Insert(index, k.Name);
                    //index++;
                }
                else
                {
                    Out.Insert(index, k.Name);
                    index++;
                    return index;
                }
            }


            if (k.LeftKnot == null)
            {
                
            }
            else
            {
                index = OutFormula(k.LeftKnot, index);
            }

            if (k.RightKnot == null)
            {
                //index++;
            }
            else
            {
                index = OutFormula(k.RightKnot, index + 1);
                if ((k.Name == "+" || k.Name == "*" || k.Name == "=>") && k.Name != "!") index++;
                //if (k.RightKnot.RightKnot == null && k.Name != "!") index++;
            }
            
            return index;
        }

        private string GetString(Knot k, string str)
        {
            str += k.Name + " ";
            if (k.LeftKnot != null) str = GetString(k.LeftKnot, str);
            if (k.RightKnot != null) str = GetString(k.RightKnot, str);
            return str;
        }

        public void DistributivityKNF(Knot k) //(a+(b*c))
        {
            if (k.Name == "+")
            {
                if (k.RightKnot.Name == "*")
                {
                    Knot a = k.LeftKnot;
                    Knot b = k.RightKnot.LeftKnot;
                    Knot c = k.RightKnot.RightKnot;

                    k.Name = "*";
                    k.LeftKnot = new Knot("+", k);
                    k.RightKnot = new Knot("+", k);

                    k.LeftKnot.LeftKnot = a;
                    k.LeftKnot.RightKnot = b;
                    k.RightKnot.LeftKnot = a;
                    k.RightKnot.RightKnot = c;


                    Kol++;
                }

                if (k.LeftKnot.Name == "*")
                {
                    Knot a = k.LeftKnot.LeftKnot;
                    Knot b = k.LeftKnot.RightKnot;
                    Knot c = k.RightKnot;

                    k.Name = "*";
                    k.LeftKnot = new Knot("+", k);
                    k.RightKnot = new Knot("+", k);

                    k.LeftKnot.LeftKnot = a;
                    k.LeftKnot.RightKnot = c;
                    k.RightKnot.LeftKnot = b;
                    k.RightKnot.RightKnot = c;


                    Kol++;
                }
            }
            if (k.LeftKnot != null) DistributivityKNF(k.LeftKnot);
            if (k.RightKnot != null) DistributivityKNF(k.RightKnot);
        }

        public void DistributivityDNF(Knot k) //(a+(b*c))
        {
            if (k.Name == "*")
            {
                if (k.RightKnot.Name == "+")
                {
                    Knot a = k.LeftKnot;
                    Knot b = k.RightKnot.LeftKnot;
                    Knot c = k.RightKnot.RightKnot;

                    k.Name = "+";
                    k.LeftKnot = new Knot("*", k);
                    k.RightKnot = new Knot("*", k);

                    k.LeftKnot.LeftKnot = a;
                    k.LeftKnot.RightKnot = b;
                    k.RightKnot.LeftKnot = a;
                    k.RightKnot.RightKnot = c;


                    Kol++;
                }

                if (k.LeftKnot.Name == "+")
                {
                    Knot a = k.LeftKnot.LeftKnot;
                    Knot b = k.LeftKnot.RightKnot;
                    Knot c = k.RightKnot;

                    k.Name = "+";
                    k.LeftKnot = new Knot("*", k);
                    k.RightKnot = new Knot("*", k);

                    k.LeftKnot.LeftKnot = a;
                    k.LeftKnot.RightKnot = c;
                    k.RightKnot.LeftKnot = b;
                    k.RightKnot.RightKnot = c;


                    Kol++;
                }
            }
            if (k.LeftKnot != null) DistributivityDNF(k.LeftKnot);
            if (k.RightKnot != null) DistributivityDNF(k.RightKnot);
        }

        public void RemoveNot(Knot k)
        {
            string str2 = k.Name;
            bool flag = true;
            if (k.Name == "!")
            {
                if (k.RightKnot.Name == "!")
                {
                    //k.RightKnot.RightKnot.Parent = k;
                    Knot kk = k.RightKnot.RightKnot;
                    k.Name = kk.Name;
                    kk.Parent = k.Parent;
                    k.RightKnot = kk.RightKnot;
                    k.LeftKnot = kk.LeftKnot;
                    flag = false;
                }
            }
            if (flag)
            {
                if (k.LeftKnot != null) RemoveNot(k.LeftKnot);
                if (k.RightKnot != null) RemoveNot(k.RightKnot);
            }
            else RemoveNot(k);
        }

        public void Not(Knot k)
        {
            bool flag = true;
            if (k.Name == "!")
            {
                string str = k.RightKnot.Name;
                if (str == "+" || str == "*")
                {
                    Knot kl = k.RightKnot.LeftKnot;
                    Knot kr = k.RightKnot.RightKnot;
                    if (str == "+") k.Name = "*";
                    else k.Name = "+";
                    k.LeftKnot = new Knot("!", k);
                    k.RightKnot = new Knot("!", k);
                    kl.Parent = k.LeftKnot;
                    kr.Parent = k.RightKnot;
                    k.LeftKnot.RightKnot = kl;
                    k.RightKnot.RightKnot = kr;
                    Kol++;
                    flag = false;
                }
            }

            if (flag)
            {
                if (k.LeftKnot != null) Not(k.LeftKnot);
                if (k.RightKnot != null) Not(k.RightKnot);
            }
            else Not(k);
        }

        public void RemoveImplication(Knot k)
        {
            if (k.Name == "=>")
            {
                k.Name = "+";
                Knot kk = k.LeftKnot;
                k.LeftKnot = new Knot("!", k);
                kk.Parent = k.LeftKnot;
                k.LeftKnot.RightKnot = kk;
            }
            if (k.LeftKnot != null) RemoveImplication(k.LeftKnot);
            if (k.RightKnot != null) RemoveImplication(k.RightKnot);
        }

        public bool Result(Knot k)
        {
            string str = k.Name;
            switch (str)
            {
                case "!":
                    return !Result(k.RightKnot);
                    break;
                case "*":
                    if (Result(k.LeftKnot) && Result(k.RightKnot)) return true;
                    else return false;
                    break;
                case "+":
                    if (Result(k.LeftKnot) || Result(k.RightKnot)) return true;
                    else return false;
                    break;
                case "=>":
                    if (Result(k.LeftKnot) && !Result(k.RightKnot)) return false;
                    else return true;
                    break;
                default:
                    return Dictionary[str];
                    break;
            }
        }

        public Knot Parsing()
        {
            string str = Formula;
            int len = str.Length;
            Knot mainKnot = new Knot("", null);
            Knot nowKnot = mainKnot;
            bool IsFormula = true;
            int depth = 1;
            for (int i = 0; i < len; i++)
            {
                if(!IsFormula) break;
                char simv = str[i];
                switch (simv)
                {
                    //(!r=>!(!!(!(!p=>!q)*r)+(p*(!q*!r))))
                    //str="((!p=>q)*((s=>(!q))+(q=>s)))"
                    //(r=>!(((!p*!q)*r)+(p*(!q*!r))))
                    case '(':
                        if (nowKnot == null)
                        {
                            IsFormula = false;
                            break;
                        }
                        nowKnot.LeftKnot = new Knot("", nowKnot);
                        nowKnot = nowKnot.LeftKnot;
                        depth++;
                        break;
                    case ')':
                        if (nowKnot == null)
                        {
                            IsFormula = false;
                            break;
                        }
                        if (i > 0 && str[i - 1] == '!')
                        {
                            IsFormula = false;
                            break;
                        }
                        nowKnot = nowKnot.Parent;
                        depth--;
                        break;
                    case '=':
                        if (i > 0 && str[i - 1] == '!')
                        {
                            IsFormula = false;
                            break;
                        }
                        if (!(i + 1 < len && str[i + 1] == '>')) IsFormula = false;
                        break;
                    case '>':
                        if (i > 0 && str[i - 1] == '!')
                        {
                            IsFormula = false;
                            break;
                        }
                        if (nowKnot == null)
                        {
                            IsFormula = false;
                            break;
                        }
                        nowKnot.Name = "=>";
                        nowKnot.RightKnot = new Knot("", nowKnot);
                        nowKnot = nowKnot.RightKnot;
                        depth++;
                        break;
                    case '+':
                        if (i > 0 && str[i - 1] == '!')
                        {
                            IsFormula = false;
                            break;
                        }
                        if (nowKnot == null)
                        {
                            IsFormula = false;
                            break;
                        }
                        nowKnot.Name = "+";
                        nowKnot.RightKnot = new Knot("", nowKnot);
                        nowKnot = nowKnot.RightKnot;
                        depth++;
                        break;
                    case '*':
                        if (i > 0 && str[i - 1] == '!')
                        {
                            IsFormula = false;
                            break;
                        }
                        if (nowKnot == null)
                        {
                            IsFormula = false;
                            break;
                        }
                        nowKnot.Name = "*";
                        nowKnot.RightKnot = new Knot("", nowKnot);
                        nowKnot = nowKnot.RightKnot;
                        depth++;
                        break;
                    case '!':
                        if (nowKnot == null)
                        {
                            IsFormula = false;
                            break;
                        }
                        nowKnot.Name = "!";
                        nowKnot.RightKnot = new Knot("", nowKnot);
                        nowKnot = nowKnot.RightKnot;
                        depth++;
                        break;
                    default:
                        if (nowKnot == null)
                        {
                            IsFormula = false;
                            break;
                        }
                        nowKnot.Name = simv.ToString();
                        if (PeremenList.IndexOf(simv.ToString()) == -1) PeremenList.Add(simv.ToString());
                        nowKnot = nowKnot.Parent;
                        depth--;
                        break;
                }

                while (true)
                {
                    if (nowKnot != null && nowKnot.Name == "!")
                    {
                        nowKnot = nowKnot.Parent;
                        depth--;
                    }
                    else break;
                }

            }
            if (depth != 0) IsFormula = false;
            if (!Check(mainKnot)) IsFormula = false;

            return IsFormula ? mainKnot : null;
        }

        public bool Check(Knot k)
        {
            if (k.Name == "") return false;
            if (k.LeftKnot != null && !Check(k.LeftKnot)) return false;
            if (k.RightKnot != null && !Check(k.RightKnot)) return false;
            return true;
        }
    }
}
