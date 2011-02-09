using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Google.GData.Client;
using Google.GData.Calendar;
using Google.GData.Extensions;
using Google.AccessControl;

namespace SampleGData
{
    public partial class Form1 : Form
    {
        //variables to cache the entries
        CalendarService myService = new CalendarService("exampleCo-exampleApp-1");

        public Form1()
        {
            InitializeComponent();
        }

        private void initCalendar()
        {
            //Logging in checks
            myService.setUserCredentials("mohit.kanwal@gmail.com", "world1-1");
        }

        private void refreshCalendar()
        {
            CalendarQuery query = new CalendarQuery();
            
            query.Uri = new Uri("https://www.google.com/calendar/feeds/default/owncalendars/full");
            CalendarFeed resultFeed = (CalendarFeed)myService.Query(query);
            //Console.WriteLine("Your calendars:\n");
            listBox1.Items.Clear();
            foreach (CalendarEntry entry in resultFeed.Entries)
            {

                listBox1.Items.Add(entry.Title.Text);
                richTextBox1.AppendText(entry.Content.AbsoluteUri.ToString() + "\n");
                //richTextBox1.Text += entry.Title.Text + "\n";
            }
        }
        
        private void createNewCalendar(CalendarService service)
        {
            CalendarEntry calendar = new CalendarEntry();
            calendar.Title.Text = textBox1.Text;//"Little League Schedule";
            calendar.Summary.Text = "This calendar contains the practice schedule and game times.";
            calendar.TimeZone = "America/Los_Angeles";
            calendar.Hidden = false;
            calendar.Color = "#2952A3";
            calendar.Location = new  Where("", "", "Oakland");

            Uri postUri = new Uri("https://www.google.com/calendar/feeds/default/owncalendars/full");
            CalendarEntry createdCalendar = (CalendarEntry)service.Insert(postUri, calendar);
            refreshCalendar();
        }

        private void deleteCalendar(CalendarService service,string calendarTitle)
        {
            //assume title is non-empty for now
            CalendarQuery query = new CalendarQuery();
            query.Uri = new Uri("https://www.google.com/calendar/feeds/default/owncalendars/full");
            CalendarFeed resultFeed = (CalendarFeed)service.Query(query);

            foreach (CalendarEntry entry in resultFeed.Entries)
            {
                if (entry.Title.Text == calendarTitle)
                {
                    try
                    {
                        entry.Delete();
                    }
                    catch (GDataRequestException)
                    {
                        MessageBox.Show("Unable to delete primary calendar.\n");
                    }
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
            // Create a CalenderService and authenticate
            initCalendar();
            refreshCalendar();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                createNewCalendar(myService);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            refreshCalendar();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                toolStripStatusLabel1.Text = "Please Select a valid Calendar";
            }
            else
            {
                string title = listBox1.Items[listBox1.SelectedIndex].ToString();
                deleteCalendar(myService, title);
                refreshCalendar();
            }
        }

        private string getUriOfSelectedCalendar(string calendarTitle)
        {
            CalendarQuery query = new CalendarQuery();

            query.Uri = new Uri("https://www.google.com/calendar/feeds/default/owncalendars/full");
            CalendarFeed resultFeed = (CalendarFeed)myService.Query(query);
            foreach (CalendarEntry entry in resultFeed.Entries)
            {
                if (calendarTitle == entry.Title.Text)
                    return entry.Content.AbsoluteUri;
            }
            return "NO";
        }
        
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                toolStripStatusLabel1.Text = "No valid calendar is selected";
            }
            else
            {
                richTextBox1.Clear();
                string calendarTitle = listBox1.Items[listBox1.SelectedIndex].ToString();
                string uri = getUriOfSelectedCalendar(calendarTitle);
                // Create the query object:
                EventQuery query = new EventQuery();
                uri.Replace("http", "https");
                query.Uri = new Uri(uri);
                //query.Uri = new Uri("https://www.google.com/calendar/feeds/mohit.kanwal@gmail.com/private/full");

                // Tell the service to query:
                EventFeed calFeed = myService.Query(query);
                //add the feature for showing events here
                foreach (EventEntry myEntry in calFeed.Entries)
                {
                    richTextBox1.AppendText("Title: " + myEntry.Title.Text);
                    foreach ( When a in myEntry.Times)
                    {
                        richTextBox1.AppendText(a.StartTime.ToString());
                    }
                    
                    richTextBox1.AppendText("\n\n");
                    
                }
                
            }
        }
    }
}
