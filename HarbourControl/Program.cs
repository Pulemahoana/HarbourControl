using HarbourControl.Enumeration;
using HarbourControl.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;


namespace HarbourControl
{
    class Program
    {
        const decimal higherWindSpeed = 30;
        const decimal lowerWindSpeed = 10;
        static int tableWidth = 73;
        static void Main(string[] args)
        {
            PrintLine();
            Console.WriteLine("            Durban Harbour             ");
            PrintLine();
            WindSpeedLookups windSpeedLookups = new WindSpeedLookups();
            var windSpeed = windSpeedLookups.GetWeatherInfo();
            Console.WriteLine($" 1) Current windspeed is:  {windSpeed}");

            var boat = GetRandomBoat();
            Console.WriteLine($" 2) Selected is: {boat}");
            PrintLine();
            BoatToEnterPerimeter(windSpeed, boat);
            SaveToDB(windSpeed, boat);
            Console.Write("\r\nPress Enter to return to Main Menu: ");
            Console.ReadLine();
            MainMenu();
        }

       

        private static void BoatToEnterPerimeter(decimal windSpeed, string boat)
        {
            if ((windSpeed > higherWindSpeed || windSpeed < lowerWindSpeed) && boat!=BoatTypes.Sailboat.ToString())
                GettingBoat(boat);
            else
                Console.WriteLine("Windspeed is not at optimum level you are not allowed to enter the perimeter, wait");
               


        }

        private static void GettingBoat(string boat)
        {
            PrintLine();
            Console.WriteLine($"{boat} is entering the perimeter");
            PrintLine();
            switch (boat)
            {
                
                case "Speedboat":
                    GoSpeedBoat();

                break;
                case "Sailboat":
                    GoSailboat();
                    break;
                case "Cargo":
                    GoCargo();
                    break;
                default:
                    Console.WriteLine("No such boat");
                    break;
            }
        }

        private static void GoCargo()
        {
            var tasks = Enumerable.Range(0, 1).Select(taskNumber => Task.Run(async () =>
            {
                Console.WriteLine("cargo going in...");
               
                await Task.Delay((taskNumber + 1) * 12000);  // cargo is lowest speed hence 12s to reach harbour
                
                Console.WriteLine("cargo reached the harbor");
            })).ToList();
          
        }

        private static void GoSailboat()
        {
            var tasks = Enumerable.Range(0, 1).Select(taskNumber => Task.Run(async () =>
            {
                Console.WriteLine("sailboat going in");
                await Task.Delay((taskNumber + 1) * 6000);  // sail boat is medium speed hence 6s to reach harbour
               
                Console.WriteLine("sailboat reached the harbor");
            })).ToList();
            

        }

        private static void GoSpeedBoat()
        {
            var tasks = Enumerable.Range(0, 1).Select(taskNumber => Task.Run(async () =>
            {
                Console.WriteLine("speedboat going in");
                await Task.Delay((taskNumber + 1) * 3000); // speed boat is faster hence 3s to reach harbour
           
                Console.WriteLine("speedboat reached the harbor");
            })).ToList();
            
        }

        private static string GetRandomBoat()
        {
            Array values = Enum.GetValues(typeof(BoatTypes));
            Random random = new Random();
            BoatTypes randomBoat = (BoatTypes)values.GetValue(random.Next(values.Length));

            return randomBoat.ToString();
        }
       
        private static bool MainMenu()
        {
            var tasks = Enumerable.Range(0, 1).Select(taskNumber => Task.Run(async () =>
            {
             await Task.Delay((taskNumber + 1) * 1000);
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1) View Added Information");
            Console.WriteLine("2) Exit");
                PrintLine();
                Console.Write("\r\nSelect an option: ");
            })).ToList();

            switch (Console.ReadLine())
            {
                case "1":
                    ViewAddedInfo();
                    return true;
  
                case "2":
                    return false;
                default:
                    return true;
            }
        }
        private static void SaveToDB(decimal windSpeed, string boat)
        {
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + "C:\\Users\\Pulem\\OneDrive\\Desktop\\HarbourControl\\DurbanHarborDB.mdf" + ";Integrated Security=True");
            con.Open();
            string boatType = boat;
            decimal wind = windSpeed;

            string query = "INSERT INTO dbo.tblDurbanHarbor(BoatType,WindSpeed) VALUES (@BoatType,@WindSpeed)";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@BoatType", boatType);
            cmd.Parameters.AddWithValue("@WindSpeed", wind);

            int i = cmd.ExecuteNonQuery();
            if (i > 0)
            {
                PrintLine();
                Console.WriteLine("Information successfully added in the Durban Harbor");
                PrintLine();
            }
            else
            {
                PrintLine();
                Console.WriteLine("No Information was added in the Durban Harbor");
                PrintLine();
            }
            con.Close();
        }

        private static void ViewAddedInfo()
        {
          
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + "C:\\Users\\Pulem\\source\\repos\\HarbourControl\\DurbanHarborDB.mdf" + ";Integrated Security=True");
            
            con.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM tblDurbanHarbor",con)
            {
                CommandType = System.Data.CommandType.Text,
                
            };
            SqlDataReader read = cmd.ExecuteReader();
           
            if (read.HasRows)
            {
                while (read.Read())
                {
                   

                    for (int i = 0; i < read.FieldCount; i++)
                    {
                        Console.Clear();
                        PrintLine();
                        Console.WriteLine("Durban Harbor Information");
                        PrintLine();
                        PrintRow("ID", "Boat Type", "Wind Speed");
                        PrintLine();        
                        PrintRow("", "", "");  
                        Console.WriteLine("\t"+ read.GetValue(i));
                        PrintRow("", "", "");
                        PrintLine();
                     
                        Console.ReadLine();


                   
                    }
                   

                }
                read.Close();
            }

        }
        static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))   
                return new string(' ', width);
            else
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            
        }
        static void PrintLine()
        {
            Console.WriteLine(new string('-', tableWidth));
        }

        static void PrintRow(params string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
        }
    }
}
