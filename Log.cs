namespace Boinkers
{
    public class Log
    {
        public static void Show(string Game, string Account, string Message, ConsoleColor Color)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"[{Game}] ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"[{Account}] ");
            Console.ForegroundColor = Color;
            Console.WriteLine(Message);
            Console.ResetColor();
        }

        public static string GetCache(string Game, string Account)
        {
            string res = string.Empty;
            try
            {
                var st = new StreamReader($@"cache\{Game}-{Account}.cache");
                res = st.ReadToEnd();
                st.Close();
            }
            catch
            {
                res = "empty";
            }

            return res;
        }

        public static void SetCache(string Game, string Account, string Value)
        {
            try
            {
                var st = new StreamWriter($@"cache\{Game}-{Account}.cache", false);
                st.Write(Value);
                st.Close();
            }
            catch { }
        }
    }
}