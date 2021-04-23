using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataClasses;

namespace Authenticator
{
    class Authenticator : AuthenticatorInterface
    {
        public const string REGISTERED_ACCOUNTS_PATH = @"D:\Harshil\Uni\Units\DC\Assignment\Registered_Accounts.txt";
        public const string ACCOUNT_TOKENS_PATH = @"D:\Harshil\Uni\Units\DC\Assignment\Account_Tokens.txt";
        //private IDictionary<int, string> tokens = new Dictionary<int, string>();
        private HashSet<int> tokens = new HashSet<int>();

        //These methods are stubs
        public string Register(string name, string password)
        {
            string ret = "unable to register";

            if(name.Equals("") || password.Equals(""))
            {
                return "Please fill out both username and password";
            }

            if(!AccountExists(name))
            {
                using (StreamWriter sw = File.AppendText(REGISTERED_ACCOUNTS_PATH))
                {
                    sw.WriteLine(name + "," + password);
                    ret = "successfully registered";
                    sw.Close();
                }
            }
            else
            {
                ret = "account already exists";
            }

            return ret;
        }

        //Change this so it only adds a token if it doesnt exist already, otherwise use the preexisting one in the file
        public string Login(string name, string password)
        {
            Random rand = new Random();
            string retval = "";
            int tok = 0;
            bool accexists = false;
            using (StreamReader sr = File.OpenText(REGISTERED_ACCOUNTS_PATH))
            {
                string[] lines = File.ReadAllLines(REGISTERED_ACCOUNTS_PATH);

                if(new FileInfo(REGISTERED_ACCOUNTS_PATH).Length == 0)
                {
                    return "There are no acounts registered yet! Please register first!";
                    //throw new FaultException<FileFormatInvalidFault>(new FileFormatInvalidFault() { Issue = "File is empty" });
                }

                //Console.WriteLine(lines[0]);
                foreach (string line in lines)
                {
                    //Console.WriteLine(line + ": " + line.Length.ToString());

                    if (line.Split(',').Length != 2)
                    {
                        return "The file: " + REGISTERED_ACCOUNTS_PATH + " was not formatted correctly";
                    }

                    string namecheck = line.Split(',')[0];
                    string passcheck = line.Split(',')[1];

                    if(namecheck.Equals(name))
                    {
                        accexists = true;
                        if(passcheck.Equals(password))
                        {
                            bool found = false;
                            using(StreamReader srtok = File.OpenText(ACCOUNT_TOKENS_PATH))
                            {
                                string[] toklines = File.ReadAllLines(ACCOUNT_TOKENS_PATH);

                                if (new FileInfo(ACCOUNT_TOKENS_PATH).Length == 0)
                                {
                                    goto NotFound;
                                }

                                //if (int.TryParse(toklines[0].Split(',')[0], out int n))
                                //{
                                //    throw new FaultException<FileFormatInvalidFault>(new FileFormatInvalidFault() { Issue = "The file: " + ACCOUNT_TOKENS_PATH + " was not formatted correctly" });
                                //}

                                foreach (string tokline in toklines)
                                {
                                    //int num;
                                    //if (int.TryParse(tokline.Split(',')[0], out num))
                                    //{
                                    //    tokens.Add(num);
                                    //}
                                    if(tokline.Split(',')[1].Equals(name))
                                    {
                                        int.TryParse(tokline.Split(',')[0], out tok);
                                        found = true;
                                    }
                                }
                                srtok.Close();
                            }    

                            NotFound:
                            if(!found)
                            {
                                tok = rand.Next(10000000, 99999999);
                                //tokens.Add(tok);
        
                                using(StreamWriter sw = File.AppendText(ACCOUNT_TOKENS_PATH))
                                {
                                    sw.WriteLine(tok + "," + name);
                                    sw.Close();
                                }
                            }
                            break;
                        }
                        else
                        {
                            return "Incorrect password, try again";
                        }
                    }
                }

                if(!accexists)
                {
                    return "Username does not exist, please use a valid username or register first";
                }

                sr.Close();

            }
            retval = tok.ToString();
            return retval;
        }

        public string Validate(int token)
        {
            string valid = "not validated";
            using (StreamReader srtok = File.OpenText(ACCOUNT_TOKENS_PATH))
            {
                string[] toklines = File.ReadAllLines(ACCOUNT_TOKENS_PATH);
                int num;
                foreach (string tokline in toklines)
                {
                    int.TryParse(tokline.Split(',')[0], out num);
                    if (num == token)
                    {
                        valid = "validated";
                    }
                }
            }
            return valid;
        }

        private bool AccountExists(string name)
        {
            bool exists = false;
            using(StreamReader sr = File.OpenText(REGISTERED_ACCOUNTS_PATH))
            {
                string[] lines = File.ReadAllLines(REGISTERED_ACCOUNTS_PATH);
                
                foreach(string line in lines)
                {
                    string namesplit = line.Split(',')[0];
                    if (namesplit.Equals(name))
                    {
                        exists = true;
                        break;
                    }
                }
                sr.Close();
            }
            return exists;
        }
    }
}
