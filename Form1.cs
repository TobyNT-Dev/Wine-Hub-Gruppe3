using DVI_Access_Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        //Set variables with API data
        private DVI_Climate climate = new DVI_Climate("http://docker.data.techcollege.dk:5051");
        private DVI_Stock stock = new DVI_Stock("http://docker.data.techcollege.dk:5051");

        //generate the window, and components
        public Form1()
        {
            //Initialize components
            InitializeComponent();

            //Start while-loops
            GetStockAsync();
            GetClimateAsync();
            GetTime();
        }

        //Get Timezones and refresh every second
        private async void GetTime()
        {
            //Start while-loop that gets the time, and sets them to labels in the UI
            while (true)
            {
                //set the local timezone
                var localZone = DateTime.Now.ToString("hh:mm:ss");

                // Get time for Washington
                var easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                var washingtonTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone).ToString("hh:mm:ss");

                // Get time for Beijing
                var chinaZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
                var beijingTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, chinaZone).ToString("hh:mm:ss");
                
                //apply the data to the labels
                label7.Text = "København \n" + localZone;
                label8.Text = "Washington DC \n" + washingtonTime;
                label9.Text = "Beijing \n" + beijingTime;
                //wait 1 second, then run again
                await Task.Delay(1000);
            }
        }
        //Get stock info, and get the needed items
        private async void GetStockAsync()
        {
            //set up the "table" dataGridView context header
            table.Columns.Add("numInStock", "Antal");
            table.Columns.Add("wineName", "Navn");
            //set The width of table
            table.Columns[0].Width = 50;
            table.Columns[1].Width = 280;
            table.ForeColor = Color.Black;

            //set up the "table2" dataGridView context header
            table2.Columns.Add("numInStock", "Antal");
            table2.Columns.Add("wineName", "Navn");
            //Set the width of table2
            table2.Columns[0].Width = 50;
            table2.Columns[1].Width = 280;
            table2.ForeColor = Color.Black;

            //Start the while loop
            while (true)
            {
                //clear old table content
                table.Rows.Clear();
                //set the StockOverThreshold as used variable
                var winesOver = await Task.Run(() => stock.StockOverThreshold());
                //Put every item in "winesOver" into the table
                foreach (Wine wine in winesOver)
                {
                    table.Rows.Add(wine.NumInStock.ToString(), wine.WineName.ToString());
                }

                //clear old table content
                table2.Rows.Clear();
                //set StockOverThreshold as used variable
                var winesUnder = await Task.Run(() => stock.StockUnderThreshold());
                //Put every item in "wineUnder" into the table
                foreach (Wine wine in winesUnder)
                {
                    table2.Rows.Add(wine.NumInStock.ToString(), wine.WineName.ToString());
                }
                //Wait 30 seconds, then run again
                await Task.Delay(30000);
            }
        }

        //Get and set climate data and refresh every 5 seconds
        private async void GetClimateAsync()
        {
            //set the loop
            while (true)
            {
                //get the API values once, invoke the action in which the value is used
                float temp = await Task.Run(() => climate.CurrTemp());
                float maxTemp = await Task.Run(() => climate.MaxTemp());
                float minTemp = await Task.Run(() => climate.MinTemp());
                //start the action when the value has been received
                label2.Invoke(new Action(() =>
                {
                    //if the temperature value is above the max, then run the warning
                    if (temp > maxTemp)
                    {
                        //alarm sound
                        Console.Beep();
                        //set color of the temperature label to red
                        label2.ForeColor = Color.DarkRed;
                        //put the value in the label
                        label2.Text = "Temperatur: " + temp.ToString("0.00") + "°";
                        //set the warning label text, and make it visible
                        warningLabel.Text = "Advarsel! Temperatur på lager er for høj.";
                        warningLabel.Visible = true;
                    }
                    // - or if the temperature is below the max, then run warning
                    else if (temp < minTemp)
                    {
                        //alarm sound
                        Console.Beep();
                        //set the color of temperature label to red
                        label2.ForeColor = Color.DarkRed;
                        //put the value in the label
                        label2.Text = "Temperatur: " + temp.ToString("0.00") + "°";
                        //set the warning label text, and make it visible
                        warningLabel.Text = "Advarsel! Temperatur på lager er for lav.";
                        warningLabel.Visible = true;
                    }
                    //if the warning conditions havent been met, then display temperature like normal
                    else
                    {
                        //set color to green
                        label2.ForeColor = Color.Green;
                        //set the label text and value
                        label2.Text = "Temperatur: " + temp.ToString("0.00") + "°";
                        //hide the warning label
                        warningLabel.Visible = false;
                    }
                }));

                //get the API values once, invoke the action in which the value is used
                float hum = await Task.Run(() => climate.CurrHum());
                float maxHum = await Task.Run(() => climate.MaxHum());
                float minHum = await Task.Run(() => climate.MinHum());
                //Start action when the value is received
                label3.Invoke(new Action(() =>
                {
                    //if the humidity value is above the max, then run the warning
                    if (hum > maxHum)
                    {
                        Console.Beep();
                        label3.ForeColor = Color.DarkRed;
                        label3.Text = "Luftfugtighed: " + hum.ToString("0.00") + "%";
                        warningLabel2.Text = "Advarsel! ´Luftfugtighed på lager er for høj.";
                        warningLabel2.Visible = true;
                    }
                    // - or if the humidity value is below the max, then run the warning
                    else if (hum < minHum)
                    {
                        Console.Beep();
                        label3.ForeColor = Color.DarkRed;
                        label3.Text = "Luftfugtighed: " + hum.ToString("0.00") + "%";
                        warningLabel2.Text = "Advarsel! Luftfugtighed på lager er for lav.";
                        warningLabel2.Visible = true;
                    }
                    //if the warning conditions havent been met, then display temperature like normal
                    else
                    {
                        label3.ForeColor = Color.Green;
                        label3.Text = "Luftfugtighed: " + hum.ToString("0.00") + "%";
                        warningLabel2.Visible = false;
                    }
                }));
                //Wait 5 seconds, then run again
                await Task.Delay(5000);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
