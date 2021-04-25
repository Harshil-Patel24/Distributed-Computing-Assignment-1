using System;
using System.IO;
using DataClasses;

namespace Authenticator
{
    //The authenticator is used to register and log in to this program
    class Authenticator : AuthenticatorInterface
    {
        //These are the locations of the stored accounts and tokens
        //Change these as needed to run
        public const string REGISTERED_ACCOUNTS_PATH = @".\\Accounts.txt";
        public const string ACCOUNT_TOKENS_PATH = @".\\Tokens.txt";

        //public const string REGISTERED_ACCOUNTS_PATH = @".\Accounts.txt";
        //public const string ACCOUNT_TOKENS_PATH = @".\Tokens.txt";

        //Registers a new user
        public string Register(string name, string password)
        {  
            //String defaults to fail until succeeded
            string ret = "unable to register";

            //Ensure that neither name or password are empty
            if(name.Equals("") || password.Equals(""))
            {
                return "Please fill out both username and password";
            }

            //Proceed if the account doesn't exist
            if(!AccountExists(name))
            {
                //Open the registered accounts file and write the name and password as comma separated values
                using (StreamWriter sw = File.AppendText(REGISTERED_ACCOUNTS_PATH))
                {
                    sw.WriteLine(name + "," + password);
                    //Change the return value to be a success
                    ret = "successfully registered";
                    sw.Close();
                }
            }
            //If the account exists return a string that informs user that it already exists
            else
            {
                ret = "account already exists";
            }
            return ret;
        }

        //Logs the user in
        public string Login(string name, string password)
        {
            //Random used to make tokens
            Random rand = new Random();
            //Retval will return token if succeeded or error messages if not
            string retval = "";
            int tok = 0;
            bool accexists = false;

            using (StreamReader sr = File.OpenText(REGISTERED_ACCOUNTS_PATH))
            {
                string[] lines = File.ReadAllLines(REGISTERED_ACCOUNTS_PATH);

                //Checks if there are any registered accounts
                if(new FileInfo(REGISTERED_ACCOUNTS_PATH).Length == 0)
                {
                    return "There are no acounts registered yet! Please register first!";
                }

                foreach (string line in lines)
                {
                    //The file must be comma separated with username and password
                    if (line.Split(',').Length != 2)
                    {
                        return "The file: " + REGISTERED_ACCOUNTS_PATH + " was not formatted correctly";
                    }

                    string namecheck = line.Split(',')[0];
                    string passcheck = line.Split(',')[1];

                    if(namecheck.Equals(name))
                    {
                        //This boolean is used to give meaningful error messages
                        accexists = true;
                        if(passcheck.Equals(password))
                        {
                            //User log in will be successful
                            //Not to check if the account already has a token, or if we need to generate one
                            bool found = false;
                            using(StreamReader srtok = File.OpenText(ACCOUNT_TOKENS_PATH))
                            {
                                string[] toklines = File.ReadAllLines(ACCOUNT_TOKENS_PATH);

                                //If this is true we need to generate a token
                                if (new FileInfo(ACCOUNT_TOKENS_PATH).Length == 0)
                                {
                                    goto NotFound;
                                }

                                //Iterate through the file and find if the user already has a token
                                foreach (string tokline in toklines)
                                {
                                    if(tokline.Split(',')[1].Equals(name))
                                    {
                                        //If token found we'll use this token instead
                                        int.TryParse(tokline.Split(',')[0], out tok);
                                        found = true;
                                    }
                                }
                                srtok.Close();
                            }    

                            NotFound:
                            //Generate a token
                            if(!found)
                            {
                                tok = rand.Next(10000000, 99999999);
        
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

        //Used to validate a token
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

        //Checks if an account exists in the registered accounts file
        public bool AccountExists(string name)
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
