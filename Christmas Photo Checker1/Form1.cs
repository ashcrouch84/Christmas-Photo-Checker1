using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using Renci.SshNet;
using System.Windows.Input;
using System.IO;
using System.Windows;
using System.Security.Cryptography;

namespace Christmas_Photo_Checker1
{
    public partial class frmCheckPhotos : Form
    {
        int i, j;
        string strText;
        List<string> ftp_list = new List<string>();

        List<string> imageName_list = new List<string>();
        List<string> imageAddress_list = new List<string>();
        List<string> imageFTP_list = new List<string>();

        List<string> family_images = new List<string>();
        List<string> adult_images = new List<string>();
        List<string> child_images = new List<string>();
        List<string> search_images = new List<string>();
        List<string> search_FTP = new List<string>();
        List<string> search_List = new List<string>();
        List<string> personSplit = new List<string>();
        List<string> picName = new List<string>();
        string strDateFrom, strDateTo;
        Image img, img1;
        int rLeft;

        int intPicCount, intSearchPicCount;
        public frmCheckPhotos()
        {
            InitializeComponent();
            loadSettings();
          
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void cmdSearch_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.phoTime == 0)
            {
                if (cboDate.Text == "" || cboTime.Text == "")
                {
                    MessageBox.Show("Please select a date and time", "Missing Variables");
                }
                else
                {
                    lblinfo1.Text = cboDate.Text + "    " + cboTime.Text;
                    findbookings();
                    i = 0;
                    cboTime.Enabled = true;
                }
            }
            if (Properties.Settings.Default.phoTime==1)
            {
                if (cboDate.Text == "" )
                {
                    MessageBox.Show("Please select a date", "Missing Variables");
                }
                else
                {
                    lblinfo1.Text = cboDate.Text + "    " + cboTime.Text;
                    findbookings();
                    i = 0;
                    cboTime.Enabled = true;
                }
            }
        }

        private void loadDates()
        {
            cboDate.Items.Clear();
            string[] DT = Properties.Settings.Default.phoFrom.ToString().Split('/');
            DateTime dd = new DateTime(Int32.Parse(DT[2]), Int32.Parse(DT[1]), Int32.Parse(DT[1]));
            string strDate = dtpFrom.Value.ToString("dd/MM/yyyy");
            while (strDate != dtpTo.Value.ToString("dd/MM/yyyy"))
            {
                cboDate.Items.Add(strDate);
                dd = dd.AddDays(1);
                strDate = dd.ToString("dd/MM/yyyy");
            }
        }

        private void loadSettings()
        {
            if (Properties.Settings.Default.phoReset==true)
            {
                rbResetYes.Checked = true;
            }
            else
            {
                rbResetNo.Checked = true;
            }

            //load dates into dob
            cboDay.Items.Clear();
            i = 0;
            while (i < 31)
            {
                cboDay.Items.Add(i.ToString());
                i = i + 1;
            }
            cboMonth.Items.Clear();
            i = 1;
            while (i < 13)
            {
                cboMonth.Items.Add(i.ToString());
                i = i + 1;
            }
            cboYear.Items.Clear();
            i = 2000;
            while (i < 2023)
            {
                cboYear.Items.Add(i.ToString());
                i = i + 1;
            }

            //combobox bits
            cboLaugh.Items.Add("Yes");
            cboLaugh.Items.Add("No");
            cboBefore.Items.Add("Yes");
            cboBefore.Items.Add("No");
            cboKeep.Items.Add("Yes");
            cboKeep.Items.Add("No");

            //load decryption password
            txtDCPW.Text = Properties.Settings.Default.phoDCPW.ToString();

            //update tabs
            if (Properties.Settings.Default.phoUsage==1)
            {
                rbDisplayOnly.Checked = true;
            }
            if (Properties.Settings.Default.phoUsage==2)
            {
                rbCustomerOnly.Checked = true;
            }
            if (Properties.Settings.Default.phoUsage==3)
            {
                rbBoth.Checked = true;
            }

            //load dates
            string[] dateFrom = Properties.Settings.Default.phoFrom.ToString().Split('/');
            string[] dateTo = Properties.Settings.Default.phoTo.ToString().Split('/');
            dtpFrom.Value = new DateTime(Int32.Parse(dateFrom[2]), Int32.Parse(dateFrom[1]), Int32.Parse(dateFrom[0]));
            dtpTo.Value = new DateTime(Int32.Parse(dateTo[2]), Int32.Parse(dateTo[1]), Int32.Parse(dateTo[0]));
            loadDates();
            updateTabs();

            //question settings
            //cboQuestionFor.Items.Add("Child");
            //cboQuestionFor.Items.Add("Adult");
            //cboQuestionFor.Items.Add("Family");
            //search settings
            gbSearch.Visible = false;
            //label clear
            lblInfo.Text = "";

            //timer settings
            timer1.Enabled = false;
            timer1.Interval = 1000;

          

            //load times
            i = 9;
            while (i < 19)
            {
                j = 0;
                while (j < 60)
                {
                    if (j == 0)
                    {
                        strText = i.ToString() + ":00";
                    }
                    else
                    {
                        strText = i.ToString() + ":" + j.ToString();
                    }
                    cboTime.Items.Add(strText);
                    j = j + 10;
                }
                i = i + 1;
            }
            i = 0;

            //load properties
            loadFTPDetails();

            txtWait.Text = Properties.Settings.Default.phoTimerInterval.ToString();

            txtLocal.Text = Properties.Settings.Default.phoSaveLocal.ToString();


            //check what way we are searching
            if (Properties.Settings.Default.phoTime == 0)
                {
                    rbTime.Checked = true;
                }
            if (Properties.Settings.Default.phoTime == 1)
            {
                rbDay.Checked = true;
            }
            loadSearchType();
            clearGroupBoxes();
            lblinfo1.Text = "";
            lblInfo.Text = "";
        }

        private void loadSearchType()
        {
            if (Properties.Settings.Default.phoTime==0)
            {
                cboTime.Visible = true;
                //gbPhotos.Text = "Photos for this time and date";
                lblTime.Visible = true;
            }
            if (Properties.Settings.Default.phoTime == 1)
            {
                cboTime.Visible = false;
               // gbPhotos.Text = "Photos for this date";
                lblTime.Visible = false;
            }
            cboDate.Text = "";
            cboTime.Text = "";
            clearGroupBoxes();
            lblInfo.Text = "";
            lstName.Items.Clear();
            lstRef.Items.Clear();

        }

        private void loadFTPDetails()
        {
            txtHost.Text = Properties.Settings.Default.phoHost.ToString();
            txtPassword.Text = Properties.Settings.Default.phoPassword.ToString();
            txtPort.Text = Properties.Settings.Default.phoPort.ToString();
            txtUsername.Text = Properties.Settings.Default.phoUsername.ToString();
            txtRFAdult.Text = Properties.Settings.Default.phoRFAdult.ToString();
            txtRFChild.Text = Properties.Settings.Default.phoRFChild.ToString();
            txtRFFamily.Text = Properties.Settings.Default.phoRFFamily.ToString();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            double x = 100 / Properties.Settings.Default.phoTimerInterval;
            x = x * i;
            lblInfo.Text = x.ToString() + "%";
            lblInfo.Refresh();
            i = i + 1;
            if (i == Properties.Settings.Default.phoTimerInterval)
            {
                timer1.Enabled = false;
                readFile();
                loadPictures();
            }
        }

        private void  readFile()
        {
            lblInfo.Text = "Downloading file";
            lblInfo.Refresh();
            //read the file created by the php function after a certain amount of time

            //variables
            string c, b;
            string strread;

            //attempt to download information from server
            string Host = Properties.Settings.Default.phoHost.ToString();
            int Port = Properties.Settings.Default.phoPort;
            string Username = Properties.Settings.Default.phoUsername.ToString();
            string Password = Properties.Settings.Default.phoPassword.ToString();

            try
            {
                ftp_list.Clear();
                using (var sftp = new SftpClient(Host, Port, Username, Password))
                {
                    sftp.Connect(); //connect to server

                    c = cboDate.Text.Replace(@"/", "") + cboTime.Text.Replace(@":","")+ "gathered.txt";
                    b = Properties.Settings.Default.phoSaveLocal.ToString() + "/" + c;
                    c = Properties.Settings.Default.phoRF.ToString() + "/" + c;
                    using (var file = File.OpenWrite(b))
                    {
                        sftp.DownloadFile(c, file);//download file
                    }
                    lblInfo.Text = "Download Complete";
                    lblInfo.Refresh();
                }
            }
            catch
            {
                MessageBox.Show("Failed to connect to ftp site, checking backups");
            }

            lblInfo.Text = "Reading downloaded file";
            lblInfo.Refresh();

            //read file
            try
            {
                var list = new List<string>();
                strread = Properties.Settings.Default.phoSaveLocal.ToString() + "/" + cboDate.Text.Replace(@"/", "") + cboTime.Text.Replace(@":", "") + "gathered.txt";
                var fileStream = new FileStream(strread, FileMode.Open, System.IO.FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        list.Add(line);
                    }
                }
                fileStream.Close();

                //parse file and display booking references and booking name
                lstRef.Items.Clear();
                lstName.Items.Clear();

                i = 0;
                while (i < list.Count())
                {
                    List<string> questionsSplit = list[i].ToString().Split(',').ToList<string>();
                    lstRef.Items.Add(questionsSplit[0].ToString());
                    lstName.Items.Add(questionsSplit[1].ToString());
                    i = i + 1;
                }
            }
            catch
            {
                MessageBox.Show("Problem reading downloaded text file", "File Error");
            }
           
        }

        private void cmdSearch_Click_1(object sender, EventArgs e)
        {
            bool bsuccess = false;

            if (Properties.Settings.Default.phoTime == 0)
            {
                if (cboDate.Text == "" || cboTime.Text == "")
                {
                    MessageBox.Show("Please select a date and time", "Missing Variables");
                }
                else
                {
                    bsuccess = true;
                }
            }
            if (Properties.Settings.Default.phoTime == 1)
            {
                if (cboDate.Text == "")
                {
                    MessageBox.Show("Please select a date", "Missing Variables");
                }
                else
                {
                    bsuccess = true;
                }
            }

            if (bsuccess==true)
            {
                intPicCount = 0;
                //clear list boxes
                lstRef.Items.Clear();
                lstName.Items.Clear();
                //empty group boxes & labels
                clearGroupBoxes();
                //update text
                lblinfo1.Text = "Families at " + cboDate.Text + " " + cboTime.Text;
                //find the bookings
                findbookings();
                i = 0;
                cboTime.Enabled = true;
                pnlFamily.Visible =false;
            }
        }

        private void cmdSaveInterval_Click(object sender, EventArgs e)
        {
            if (txtWait.Text == "")
            { }
            else
            {
                try
                {
                    Properties.Settings.Default.phoTimerInterval = Int32.Parse(txtWait.Text);
                    Properties.Settings.Default.Save();
                }
                catch
                {
                    MessageBox.Show("Please ensure the interval in a valid number", "Interval Error");
                }
            }
        }

        private void lstName_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstRef.SelectedIndex = lstName.SelectedIndex;
        }

        private void loadPictures()
        {
            lblInfo.Text = "Downloading photos";
            lblInfo.Refresh();
            //variables

            string Host = Properties.Settings.Default.phoHost;
            int Port = Properties.Settings.Default.phoPort;
            string Username = Properties.Settings.Default.phoUsername;
            string Password = Properties.Settings.Default.phoPassword;

            string c, b, ga, gc, gf;

            string strRemoteFolderC = Properties.Settings.Default.phoRFChild.ToString();
            List<string> child_list = new List<string>();

            string strRemoteFolderA = Properties.Settings.Default.phoRFChild.ToString();
            List<string> adult_list = new List<string>();

            string strRemoteFolderF = Properties.Settings.Default.phoRFFamily.ToString();
            List<string> family_list = new List<string>();
            

            gf = Properties.Settings.Default.phoSaveLocal + "\\Family Photos";
            ga = Properties.Settings.Default.phoSaveLocal + "\\Adult Photos";
            gc = Properties.Settings.Default.phoSaveLocal + "\\child Photos";

            imageName_list.Clear();
            imageAddress_list.Clear();
            imageFTP_list.Clear();
            adult_images.Clear();
            child_images.Clear();
            family_images.Clear();

            int x = 0;

            //clear group boxes
            clearGroupBoxes();

            //connect to server
                ftp_list.Clear();
                using (var sftp = new SftpClient(Host, Port, Username, Password))
                {
                    sftp.Connect(); //connect to server

                    //add every file in each directory to a list
                    child_list = sftp.ListDirectory(Properties.Settings.Default.phoRFChild).Where(f => !f.IsDirectory).Select(f => f.Name).ToList();
                    adult_list = sftp.ListDirectory(Properties.Settings.Default.phoRFAdult).Where(f => !f.IsDirectory).Select(f => f.Name).ToList();
                    family_list = sftp.ListDirectory(Properties.Settings.Default.phoRFFamily).Where(f => !f.IsDirectory).Select(f => f.Name).ToList();

                    //create a adult folder if it doesnt exist and delete all files inside it
                    System.IO.Directory.CreateDirectory(ga);
                    DirectoryInfo dir = new DirectoryInfo(ga);
                    foreach (FileInfo fi in dir.GetFiles())
                    {
                    try
                    {
                        fi.Delete();
                    }
                    catch
                    { }
                }

                    //create a child folder if it doesnt exist and delete all files inside it

                    System.IO.Directory.CreateDirectory(gc);
                    dir = new DirectoryInfo(gc);
                    foreach (FileInfo fi in dir.GetFiles())
                    {
                        try
                        {
                            fi.Delete();
                        }
                        catch
                        { }
                    }

                    //create a family folder if it doesnt exist and delete all files inside it
                    System.IO.Directory.CreateDirectory(gf);
                    dir = new DirectoryInfo(gf);
                    foreach (FileInfo fi in dir.GetFiles())
                    {
                        try
                        {
                            fi.Delete();
                        }
                        catch
                        { }
                    }

                    //cycle through all references
                    while (x < lstRef.Items.Count)
                    {
                        //find pictures in adult folder
                            i = 0;
                            while (i < adult_list.Count)
                            {
                                //cycle through the list and if it contains the reference number then down the picture
                                if (adult_list[i].ToString().Contains(lstRef.Items[x].ToString()) && adult_list[i].ToString().Contains(".jpg"))
                                {
                                    adult_images.Add(adult_list[i]);
                                    imageName_list.Add(adult_list[i]);
                                    imageAddress_list.Add(ga + "\\" + adult_list[i]);
                                    imageFTP_list.Add(Properties.Settings.Default.phoRFAdult + "/" + adult_list[i]);
                                    c = Properties.Settings.Default.phoRFAdult + "/" + adult_list[i]; //update download file from sftp
                                    b = ga + "\\" + adult_list[i];//update download folder to pc 
                                    try
                                    {
                                        using (var file = File.OpenWrite(b))
                                        {
                                            sftp.DownloadFile(c, file);//download file
                                        }
                                    }
                                    catch
                                    { }
                                }
                                i = i + 1;
                            }

                        //find pictures in child folder
                            i = 0;
                            while (i < child_list.Count)
                            {
                                //cycle through the list and if it contains the reference number then down the picture
                                if (child_list[i].ToString().Contains(lstRef.Items[x].ToString()) && child_list[i].ToString().Contains(".jpg"))
                                {
                                    child_images.Add(child_list[i]);
                                    imageName_list.Add(child_list[i]);
                                    imageAddress_list.Add(gc + "\\" + child_list[i]);
                                    imageFTP_list.Add(Properties.Settings.Default.phoRFChild + "/" + child_list[i]);
                                    c = Properties.Settings.Default.phoRFChild + "/" + child_list[i]; //update download file from sftp
                                    b = gc + "\\" + child_list[i];//update download folder to pc 
                                    try
                                    {
                                        using (var file = File.OpenWrite(b))
                                        {
                                            sftp.DownloadFile(c, file);//download file
                                        }
                                    }
                                    catch
                                    { }   
                                }
                                i = i + 1;
                            }
                       

                        //find pictures in family folder
                            i = 0;
                            while (i < family_list.Count)
                            {
                                //cycle through the list and if it contains the reference number then down the picture
                                if (family_list[i].ToString().Contains(lstRef.Items[x].ToString()) && family_list[i].ToString().Contains(".jpg"))
                                {
                                    family_images.Add(family_list[i]);
                                    imageName_list.Add(family_list[i]);
                                    imageAddress_list.Add(gf + "\\" + family_list[i]);
                                    imageFTP_list.Add(Properties.Settings.Default.phoRFFamily + "/" + family_list[i]);
                                    c = Properties.Settings.Default.phoRFFamily + "/" + family_list[i]; //update download file from sftp
                                    b = gf + "\\" + family_list[i];//update download folder to pc 
                                    try
                                    {
                                        using (var file = File.OpenWrite(b))
                                        {
                                            sftp.DownloadFile(c, file);//download file
                                        }
                                    }
                                    catch
                                    { }
                                }
                                i = i + 1;
                            }
                        x = x + 1;
                    }
                }

            //display pictures
            displayPictures();
        }

        private void displayPictures()
        {
            lblInfo.Text = "Displaying Pictures";
            lblInfo.Refresh();
            intPicCount = 0;

            int intx = 16;
            int inty = 25;

            string gf = Properties.Settings.Default.phoSaveLocal + "/Family Photos";
            string ga = Properties.Settings.Default.phoSaveLocal + "/Adult Photos";
            string gc = Properties.Settings.Default.phoSaveLocal + "/child Photos";
            //Dynamically create pictureboxes and load adult pictures
            i = 0;
            while (i < adult_images.Count)
            {
                PictureBox pba = new PictureBox();
                pba.Name = "pic" + intPicCount.ToString();
                pba.Click += new EventHandler(NewButton_Click);
                pba.Location = new System.Drawing.Point(intx, inty);
                pba.Size = new System.Drawing.Size(100, 100);

                using (FileStream stream = new FileStream(ga + "//"+adult_images[i], FileMode.Open, FileAccess.Read))
                {
                    img1 = Image.FromStream(stream);
                }
                pba.BackgroundImage = img1;
                pba.BackgroundImageLayout = ImageLayout.Stretch;
                pnlPhotos.Controls.Add(pba);

                i = i + 1;
                intx = intx + 110;
                if (intx > 550)
                {
                    intx = 16;
                    inty = inty + 110;
                }
                intPicCount = intPicCount + 1;
            }

            //Dynamically create pictureboxes and load child pictures
            i = 0;
            while (i < child_images.Count)
            {
                PictureBox pbc = new PictureBox();
                pbc.Name = "pic" + intPicCount.ToString();
                pbc.Click += new EventHandler(NewButton_Click);
                pbc.Location = new System.Drawing.Point(intx, inty);
                pbc.Size = new System.Drawing.Size(100, 100);

                using (FileStream stream = new FileStream(gc + "//" + child_images[i], FileMode.Open, FileAccess.Read))
                {
                    img1 = Image.FromStream(stream);
                }
                pbc.BackgroundImage = img1;
                pbc.BackgroundImageLayout = ImageLayout.Stretch;
                pnlPhotos.Controls.Add(pbc);

                i = i + 1;
                intx = intx + 110;
                if (intx > 550)
                {
                    intx = 16;
                    inty = inty + 110;
                }
                intPicCount = intPicCount + 1;
            }

            //Dynamically create pictureboxes and load family pictures
            i = 0;
            while (i < family_images.Count)
            {
                PictureBox pbf = new PictureBox();
                pbf.Name = "pic" + intPicCount.ToString();
                pbf.Click += new EventHandler(NewButton_Click);
                pbf.Location = new System.Drawing.Point(intx, inty);
                pbf.Size = new System.Drawing.Size(100, 100);

                using (FileStream stream = new FileStream(gf + "//" + family_images[i], FileMode.Open, FileAccess.Read))
                {
                    img1 = Image.FromStream(stream);
                }
                pbf.BackgroundImage = img1;
                pbf.BackgroundImageLayout = ImageLayout.Stretch;
                pnlPhotos.Controls.Add(pbf);

                i = i + 1;
                intx = intx + 110;
                if (intx > 550)
                {
                    intx = 16;
                    inty = inty + 110;
                }
                intPicCount = intPicCount + 1;
            }

            lblInfo.Text = "Complete";
            lblInfo.Refresh();
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            PictureBox btn = (PictureBox)sender;
           // Button btn = (Button)sender;

            for (int i = 0; i < intPicCount; i++)
            {
                if (btn.Name == ("pic" + i))
                {
                    gbFound.Visible = true;
                    //display picture in large size
                    picBoxLarge.Visible = true;
                    picBoxLarge.BackgroundImage = Image.FromFile(imageAddress_list[i]);
                    picBoxLarge.BackgroundImageLayout = ImageLayout.Stretch;

                    //Display local address of the file
                    lblPhotoLocalAddress.Text = imageAddress_list[i].ToString();
                    lblPhotoLocalAddress.Visible = true;

                    //display the address on the ftp site
                    lblPhotoFTPAddress.Visible = true;
                    lblPhotoFTPAddress.Text = imageFTP_list[i].ToString();
                    
                    //find the reference number
                    lblSelRef.Visible = true;
                    List<string> questionsSplit = imageName_list[i].ToString().Split('_').ToList<string>();
                    string strRef = questionsSplit[2];
                    strRef = strRef.Substring(0, strRef.Length - 4);
                    lblSelRef.Text = strRef.ToString();

                    //display the selected family name
                    lblFamilySel.Visible = true;
                    int x = 0;
                    while (x<lstRef.Items.Count)
                    {
                        if (lstRef.Items[x].ToString() == strRef.ToString())
                        {
                            lblFamilySel.Text = lstName.Items[x].ToString();
                            x = lstRef.Items.Count;
                        }
                        x = x + 1;
                    }
 
                    //display the name of the file
                    lblNameSel.Visible = true;
                    lblNameSel.Text = imageName_list[i].ToString();

                    //display the type of photo eg, adult childhood photo, etc.
                    lblTypeSel.Visible = true;
                    lblTypeSel.Text = "";
                    //adult  photos
                    if (lblNameSel.ToString().Contains("Childhood") == true) {lblTypeSel.Text = "Adult childhood picture Photo";}
                    if (lblNameSel.ToString().Contains("ChristmasToy") == true) { lblTypeSel.Text = "Adult Childhood present Photo"; }
                    if (lblNameSel.ToString().Contains("Maintenance") == true) { lblTypeSel.Text = "Adult maintenace Photo"; }
                    //child photos
                    if (lblNameSel.ToString().Contains("Port") == true) { lblTypeSel.Text = "Child Portrait Picture"; }
                    if (lblNameSel.ToString().Contains("Pres") == true) { lblTypeSel.Text = "Child Present Picture"; }
                    if (lblNameSel.ToString().Contains("Achieve") == true) { lblTypeSel.Text = "Child Achievement Picture"; }
                    if (lblNameSel.ToString().Contains("Famous") == true) { lblTypeSel.Text = "Child Famous Person Picture"; }
                    //Family Photos
                    if (lblNameSel.ToString().Contains("Holiday") == true) { lblTypeSel.Text = "Family Holiday Photo"; }
                    if (lblNameSel.ToString().Contains("Group") == true) { lblTypeSel.Text = "Family Group Photo"; }

                    //display the name of the photo is of
                    lblPersonName.Visible = true;
                    lblPersonName.Text = questionsSplit[0].ToString();
                    if (lblPersonName.Text == "Family") { lblPersonName.Text = ""; }

                    //enable the delete button
                    cmdDel.Visible = true;

                    //display labels
                    label14.Visible = true;
                    label15.Visible = true;
                    label16.Visible = true;
                    label17.Visible = true;
                    label18.Visible = true;
                    label19.Visible = true;
                    label20.Visible = true;
                    cmdRotateCheck.Visible = true;
                }
            }


        } 

        private void clearGroupBoxes()
        {
            //gbPhotos.Controls.Clear();
            pnlPhotos.Controls.Clear();
            cmdDel.Visible=false;
            lblPhotoLocalAddress.Visible=false;
            picBoxLarge.Visible = false;
            lblPhotoFTPAddress.Visible=false;
            lblSelRef.Visible = false;
            lblNameSel.Visible = false;
            lblTypeSel.Visible = false;
            lblFamilySel.Visible = false;
            lblPersonName.Visible = false;
            label14.Visible = false;
            label15.Visible = false;
            label16.Visible = false;
            label17.Visible = false;
            label18.Visible = false;
            label19.Visible = false;
            label20.Visible = false;
            cmdRotateCheck.Visible = false;
        }

        private void lstRef_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstName.SelectedIndex = lstRef.SelectedIndex;
        }

        private void cmdSaveFTP_Click(object sender, EventArgs e)
        {
            //check connection to ftp site
            string Host = Properties.Settings.Default.phoHost.ToString();
            int Port = Properties.Settings.Default.phoPort;
            string Username = Properties.Settings.Default.phoUsername.ToString();
            string Password = Properties.Settings.Default.phoPassword.ToString();

            try
            {
                ftp_list.Clear();
                using (var sftp = new SftpClient(Host, Port, Username, Password))
                {
                    sftp.Connect(); //connect to server
                    sftp.Disconnect();
                }
                //need to check if the folders exists//
                Properties.Settings.Default.phoHost = txtHost.Text;
                Properties.Settings.Default.phoPassword = txtPassword.Text;
                Properties.Settings.Default.phoUsername = txtUsername.Text;
                Properties.Settings.Default.phoPort = Int32.Parse(txtPort.Text);
                Properties.Settings.Default.Save();

            }
            catch
            {
                MessageBox.Show("Failed to connect to ftp site, checking backups");
            }
        }

        private void cmdCancelFTP_Click(object sender, EventArgs e)
        {
            loadFTPDetails();
        }

        private void cmdDel_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to delete this picture? You Cannot recover it afterwards!!!", "Delete Photo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                //variables

                string Host = Properties.Settings.Default.phoHost;
                int Port = Properties.Settings.Default.phoPort;
                string Username = Properties.Settings.Default.phoUsername;
                string Password = Properties.Settings.Default.phoPassword;

                //connect to server and delete file
                using (var sftp = new SftpClient(Host, Port, Username, Password))
                {
                    sftp.Connect(); //connect to server
                    sftp.DeleteFile(lblPhotoFTPAddress.Text);
                }

                intPicCount = 0;
                //clear list boxes
                lstRef.Items.Clear();
                lstName.Items.Clear();
                lblInfo.Text = "Refreshing images, please wait";
                lblInfo.Refresh();
                clearGroupBoxes();
                findbookings();
            }
        }

        private void cmdBrowseSave_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
               txtLocal.Text = folderBrowserDialog1.SelectedPath;
                Properties.Settings.Default.phoSaveLocal = folderBrowserDialog1.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }

        private void rbTime_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.phoTime = 0;
            Properties.Settings.Default.Save();
            loadSearchType();
        }

        private void rbDay_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.phoTime = 1;
            Properties.Settings.Default.Save();
            loadSearchType();
        }

        private void lstRef_Click(object sender, EventArgs e)
        {
            lstName.SelectedIndex = lstRef.SelectedIndex;
            gbFound.Visible = false;
            showFamilyPictures();
            
        }

        private void lstName_Click(object sender, EventArgs e)
        {
            lstRef.SelectedIndex = lstName.SelectedIndex;
            gbFound.Visible = false;
            pnlFamily.Controls.Clear();
            showFamilyPictures();
        }

        private void showFamilyPictures()
        {

            cmdClosePanel.Visible = true;
            intPicCount = 0;
            pnlFamily.Visible = true;
            pnlPhotos.Visible = false;
            //get reference number
            string strRef = lstRef.Items[lstRef.SelectedIndex].ToString();
            //find pictures with the corrisponding reference number
            lblInfo.Text = "Displaying Pictures";
            lblInfo.Refresh();

            int intx = 16;
            int inty = 25;

            string gf = Properties.Settings.Default.phoSaveLocal + "/Family Photos";
            string ga = Properties.Settings.Default.phoSaveLocal + "/Adult Photos";
            string gc = Properties.Settings.Default.phoSaveLocal + "/child Photos";
            //Dynamically create pictureboxes and load adult pictures
            i = 0;
            while (i < adult_images.Count)
            {
                if (adult_images[i].ToString().Contains(strRef))
                {
                    PictureBox pba = new PictureBox();
                    pba.Name = "pic" + intPicCount.ToString();
                    pba.Click += new EventHandler(NewButton_Click);
                    pba.Location = new System.Drawing.Point(intx, inty);
                    pba.Size = new System.Drawing.Size(100, 100);

                    Image imgNew = Image.FromFile(ga + "//" + adult_images[i]);
                    pba.BackgroundImage = imgNew;


                    pba.BackgroundImageLayout = ImageLayout.Stretch;
                    pnlFamily.Controls.Add(pba);
                   
                    intx = intx + 110;
                    if (intx > 550)
                    {
                        intx = 16;
                        inty = inty + 110;
                    }
                    intPicCount = intPicCount + 1;
                }
                i = i + 1;
            }

            //Dynamically create pictureboxes and load child pictures
            i = 0;
            while (i < child_images.Count)
            {
                if (child_images[i].ToString().Contains(strRef))
                {
                    PictureBox pbc = new PictureBox();
                    pbc.Name = "pic" + intPicCount.ToString();
                    pbc.Click += new EventHandler(NewButton_Click);
                    pbc.Location = new System.Drawing.Point(intx, inty);
                    pbc.Size = new System.Drawing.Size(100, 100);

                    Image imgNew = Image.FromFile(gc + "//" + child_images[i]);
                    pbc.BackgroundImage = imgNew;

                    pbc.BackgroundImageLayout = ImageLayout.Stretch;
                    pnlFamily.Controls.Add(pbc);
                  
                    intx = intx + 110;
                    if (intx > 550)
                    {
                        intx = 16;
                        inty = inty + 110;
                    }
                    intPicCount = intPicCount + 1;
                }
                i = i + 1;
            }

            //Dynamically create pictureboxes and load family pictures
            i = 0;
            while (i < family_images.Count)
            {
                if (family_images[i].ToString().Contains(strRef))
                {
                    PictureBox pbf = new PictureBox();
                    pbf.Name = "pic" + intPicCount.ToString();
                    pbf.Click += new EventHandler(NewButton_Click);
                    pbf.Location = new System.Drawing.Point(intx, inty);
                    pbf.Size = new System.Drawing.Size(100, 100);

                    Image imgNew = Image.FromFile(gf + "//" + family_images[i]);
                    pbf.BackgroundImage = imgNew;


                    pbf.BackgroundImageLayout = ImageLayout.Stretch;
                    pnlFamily.Controls.Add(pbf);
               
                    intx = intx + 110;
                    if (intx > 550)
                    {
                        intx = 16;
                        inty = inty + 110;
                    }
                    intPicCount = intPicCount + 1;
                }
                i = i + 1;
                
            }
            lblInfo.Text = "Complete";
            lblInfo.Refresh();
        }

        private void lblTypeSel_Click(object sender, EventArgs e)
        {

        }

        private void txtHost_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmdClosePanel_Click(object sender, EventArgs e)
        {
            pnlFamily.Visible=false;
            pnlPhotos.Visible = true;
            pnlFamily.Controls.Clear();
            lstRef.SelectedIndex = -1;
            lstName.SelectedIndex = -1;
            cmdClosePanel.Visible = false;
            gbFound.Visible = false;
        }

        private void cmdSearchRef_Click(object sender, EventArgs e)
        {
            searchForFamily();
        }

        private void searchForFamily()
        {
            if (txtSearchRef.Text == "")
            {
                MessageBox.Show("Please enter a reference number first");
            }
            else
            {
                findFamilyPhotos();
                getDetails();
            }
        }

        private void getDetails()
        {
            if (Properties.Settings.Default.phoTime == 0)
            {
                String urlstr = "https://4k-photos.co.uk/familyData.php?ref=" + txtSearchRef.Text;
                WebClient client = new WebClient();
                System.IO.Stream response = client.OpenRead(urlstr);
                System.IO.StreamReader reads = new System.IO.StreamReader(response);
            }


            lblSearchInfo.Text = "Downloading file";
            lblSearchInfo.Refresh();
            //read the file created by the php function after a certain amount of time

            //variables
            string c, b;
            string strread;

            //attempt to download information from server
            string Host = Properties.Settings.Default.phoHost.ToString();
            int Port = Properties.Settings.Default.phoPort;
            string Username = Properties.Settings.Default.phoUsername.ToString();
            string Password = Properties.Settings.Default.phoPassword.ToString();

           try
            {
                ftp_list.Clear();
                using (var sftp = new SftpClient(Host, Port, Username, Password))
                {
                    sftp.Connect(); //connect to server

                    c = txtSearchRef.Text;
                    b = Properties.Settings.Default.phoSaveLocal.ToString() + "/" + c +".txt";
                    c = Properties.Settings.Default.phoRF.ToString() + "/" + c + ".txt";
                    using (var file = File.OpenWrite(b))
                    {
                        sftp.DownloadFile(c, file);//download file
                    }
                    lblSearchInfo.Text = "Download Complete";
                    lblSearchInfo.Refresh();
                }
            }
           catch
           {
                MessageBox.Show("Failed to connect to ftp site, checking backups");
           }

            lblSearchInfo.Text = "Reading downloaded file";
            lblSearchInfo.Refresh();
            cboPhotoOf.Items.Clear();
            cboName.Items.Clear();
            //read file
            try
            {
                var list = new List<string>();
                strread = Properties.Settings.Default.phoSaveLocal.ToString() + "/" + txtSearchRef.Text +".txt";
                var fileStream = new FileStream(strread, FileMode.Open, System.IO.FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        list.Add(line);
                    }
                }
                fileStream.Close();

                i = 0;
                while (i < list.Count())
                {
                    personSplit.Add(list[i]);
                    List<string> questionSplit = list[i].ToString().Split(',').ToList<string>();
                    cboPhotoOf.Items.Add(questionSplit[1].ToString());
                    string strModified = questionSplit[0].ToString().Substring(0, 5);
                    cboName.Items.Add(questionSplit[1].ToString() + " - " + strModified);
                    i = i + 1;
                }
                cboPhotoOf.Items.Add("Family");
                cboName.Items.Add("Family");
            }
            catch
            {
                MessageBox.Show("Problem reading downloaded text file", "File Error");
            }
        }

        private void findFamilyPhotos()
        {
            pnlSearch.Controls.Clear();
            lblSearchInfo.Text = "Searching for family";
            gbSearch.Visible = false;
            lblInfo.Refresh();
            //variables

            intSearchPicCount = 0;
            search_images.Clear();
            search_FTP.Clear();
            search_List.Clear();

            string Host = Properties.Settings.Default.phoHost;
            int Port = Properties.Settings.Default.phoPort;
            string Username = Properties.Settings.Default.phoUsername;
            string Password = Properties.Settings.Default.phoPassword;

            string c, b, gs,strRef;

            string strRemoteFolderC = Properties.Settings.Default.phoRFChild.ToString();
            List<string> child_list = new List<string>();

            string strRemoteFolderA = Properties.Settings.Default.phoRFChild.ToString();
            List<string> adult_list = new List<string>();

            string strRemoteFolderF = Properties.Settings.Default.phoRFFamily.ToString();
            List<string> family_list = new List<string>();

            strRef = txtSearchRef.Text;
            gs = Properties.Settings.Default.phoSaveLocal + "\\Searched Photos";


            imageName_list.Clear();
            imageAddress_list.Clear();
            imageFTP_list.Clear();
            adult_images.Clear();
            child_images.Clear();
            family_images.Clear();

            int x = 0;

            //clear group boxes
            clearGroupBoxes();

            //connect to server
            ftp_list.Clear();
            using (var sftp = new SftpClient(Host, Port, Username, Password))
            {
                sftp.Connect(); //connect to server

                //add every file in each directory to a list
                child_list = sftp.ListDirectory(Properties.Settings.Default.phoRFChild).Where(f => !f.IsDirectory).Select(f => f.Name).ToList();
                adult_list = sftp.ListDirectory(Properties.Settings.Default.phoRFAdult).Where(f => !f.IsDirectory).Select(f => f.Name).ToList();
                family_list = sftp.ListDirectory(Properties.Settings.Default.phoRFFamily).Where(f => !f.IsDirectory).Select(f => f.Name).ToList();

                search_images.Clear();

                //create a search folder if it doesnt exist and delete all files inside it
                System.IO.Directory.CreateDirectory(gs);
                DirectoryInfo dir = new DirectoryInfo(gs);
                foreach (FileInfo fi in dir.GetFiles())
                {
                   try
                   {
                        fi.Delete();
                    }
                    catch
                   { }
                }

                //download adult pictures from ftp site
                i = 0;
                while (i < adult_list.Count)
                {
                    if (adult_list[i].ToString().Contains(strRef) && adult_list[i].ToString().Contains(".jpg"))
                    {
                        c = Properties.Settings.Default.phoRFAdult + "/" + adult_list[i]; //update download file from sftp
                        search_FTP.Add(c);
                        search_List.Add(adult_list[i]);
                        b = gs + "\\" + adult_list[i];//update download folder to pc 
                        search_images.Add(b);
                        try
                        {
                            using (var file = File.OpenWrite(b))
                            {
                                sftp.DownloadFile(c, file);//download file
                            }
                        }
                        catch
                        { }
                    }
                    i = i + 1;
                }
                
                //download child pictures from ftp site
                i = 0;
                while (i < child_list.Count)
                {
                    if (child_list[i].ToString().Contains(strRef) && child_list[i].ToString().Contains(".jpg"))
                    {
                        c = Properties.Settings.Default.phoRFChild + "/" + child_list[i]; //update download file from sftp
                        search_FTP.Add(c);
                        search_List.Add(child_list[i]);
                        b = gs + "\\" + child_list[i];//update download folder to pc 
                        search_images.Add(b);
                        try
                        {
                            using (var file = File.OpenWrite(b))
                            {
                                sftp.DownloadFile(c, file);//download file
                            }
                        }
                        catch
                        { }
                    }
                    i = i + 1;
                }

                //download family pictures from ftp site
                i = 0;
                while (i < family_list.Count)
                {
                    if (family_list[i].ToString().Contains(strRef) && family_list[i].ToString().Contains(".jpg"))
                    {
                        c = Properties.Settings.Default.phoRFFamily + "/" + family_list[i]; //update download file from sftp
                        search_FTP.Add(c);
                        search_List.Add(family_list[i]);
                        b = gs + "\\" + family_list[i];//update download folder to pc 
                        search_images.Add(b);
                        try
                        {
                            using (var file = File.OpenWrite(b))
                            {
                                sftp.DownloadFile(c, file);//download file
                            }
                        }
                        catch
                        { }
                    }
                    i = i + 1;
                }

                //display search pictures
                displaySearchedPictures();

                cmdSearchUpload.Visible = true;
                cmdClear.Visible = true;
                cmdSearchQuestions.Visible = true;
            }
        }

        private void displaySearchedPictures()
        {
            string strRef = txtSearchRef.Text;
            intSearchPicCount = 0;
            //display pictures
            int intx = 16;
            int inty = 25;
            //Dynamically create pictureboxes and load adult pictures
            i = 0;
            while (i < search_images.Count)
            {
                if (search_images[i].ToString().Contains(strRef))
                {
                    PictureBox pbs = new PictureBox();
                    pbs.Name = "Search" + intSearchPicCount.ToString();
                    pbs.Click += new EventHandler(searchPicClick);
                    pbs.Location = new System.Drawing.Point(intx, inty);
                    pbs.Size = new System.Drawing.Size(100, 100);

                    using (FileStream stream = new FileStream(search_images[i], FileMode.Open, FileAccess.Read))
                    {
                        img1 = Image.FromStream(stream);
                    }
                    pbs.BackgroundImage = img1;
                    pbs.BackgroundImageLayout = ImageLayout.Stretch;
                    pnlSearch.Controls.Add(pbs);

                    intx = intx + 110;
                    if (intx > 550)
                    {
                        intx = 16;
                        inty = inty + 110;
                    }
                    intSearchPicCount = intSearchPicCount + 1;
                }
                i = i + 1;
            }
            pnlSearch.Visible = true;
        }

        private void cmdSearchDelete_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to delete this picture? You Cannot recover it afterwards!!!", "Delete Photo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                //variables

                string Host = Properties.Settings.Default.phoHost;
                int Port = Properties.Settings.Default.phoPort;
                string Username = Properties.Settings.Default.phoUsername;
                string Password = Properties.Settings.Default.phoPassword;

                //connect to server and delete file
                using (var sftp = new SftpClient(Host, Port, Username, Password))
                {
                    sftp.Connect(); //connect to server
                    sftp.DeleteFile(lblSearchFTPAddress.Text);
                }

                findFamilyPhotos();
            }
        }

        private void cmdSearchUpload_Click(object sender, EventArgs e)
        {
            // open file dialog   

            OpenFileDialog open = new OpenFileDialog();
            // image filters  
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (open.ShowDialog() == DialogResult.OK)
            {
                // display image in picture box  
                img=  new Bitmap(open.FileName);
                picUpload.BackgroundImage = img;
                // image file path  
                lblUploadFilename.Text = open.FileName;
                gbUpload.Visible = true;
                gbSearch.Visible = false;
                gbQuestions.Visible = false;
                cboPhotoType.Enabled = false;
            }
        }

        private void cmdRotateLeft_Click(object sender, EventArgs e)
        {
            //create a search folder if it doesnt exist and delete all files inside it
            System.IO.Directory.CreateDirectory(Properties.Settings.Default.phoSaveLocal + "\\Searched Photos\\Temp");
            DirectoryInfo dir = new DirectoryInfo(Properties.Settings.Default.phoSaveLocal + "\\Searched Photos\\Temp");
            foreach (FileInfo fi in dir.GetFiles())
            {
                try
                {
                    fi.Delete();
                }
                catch
                { }
            }

            string strOutput = Path.GetFileName(lblUploadFilename.Text);
            string[] words = lblUploadFilename.Text.Split('.');
            Random rnd = new Random();
            int rand = rnd.Next(11, 99);
            strOutput = Properties.Settings.Default.phoSaveLocal.ToString() + "\\Searched Photos\\temp\\Temp" +rand.ToString()+ strOutput;

            //rotate the image and save the new file
            using (Image img = Image.FromFile(lblUploadFilename.Text))
            {
                try
                {
                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    img.Save(strOutput, System.Drawing.Imaging.ImageFormat.Jpeg);
                    img.Dispose();
                }
                catch
                { }
            }

            picUpload.BackgroundImage = Image.FromFile(strOutput);
            lblUploadFilename.Text = strOutput;
        }

        private void cboPhotoOf_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboPhotoOf.SelectedIndex > -1)
            {
                cboPhotoType.Items.Clear();
                cboPhotoType.Enabled = true;
                picName.Clear();
                if (cboPhotoOf.Text == "Family")
                {
                    cboPhotoType.Items.Add("Family Holiday Picture");
                    picName.Add("_Holiday_");
                    cboPhotoType.Items.Add("Family_Group_");
                    picName.Add("_Group_");
                }
                else
                {
                    List<string> questionSplit = personSplit[cboPhotoOf.SelectedIndex].ToString().Split(',').ToList<string>();
                    if (questionSplit[0] == "Adult Name")
                    {
                        cboPhotoType.Items.Add("Childhood Picture");
                        picName.Add("_Childhood_");
                        cboPhotoType.Items.Add("Childhood Toy");
                        picName.Add("_ChristmasToy_");
                        cboPhotoType.Items.Add("Doing Mainteance");
                        picName.Add("_Maintenance_");
                    }
                    if (questionSplit[0] == "Child's Name")
                    {
                        cboPhotoType.Items.Add("Portrait Picture");
                        picName.Add("_Port_");
                        cboPhotoType.Items.Add("Present Picture");
                        picName.Add("_Pres_");
                        cboPhotoType.Items.Add("Achievement Picture");
                        picName.Add("_Achieve_");
                        cboPhotoType.Items.Add("Famous Picture");
                        picName.Add("_Famous_");
                    }
                }
                   
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            gbUpload.Visible = false;
            cboPhotoType.Enabled=false;
            cboPhotoOf.SelectedIndex = -1;
            cboPhotoOf.Text = "";
        }

        private void cmdUpload_Click(object sender, EventArgs e)
        {
            img1.Dispose();
            rLeft = 1;
            string strTemp;
            string targetDirectory = "";
            bool uSuccess = true;
            if ( cboPhotoOf.Text == "" || cboPhotoType.Text=="")
            {
                MessageBox.Show("Please select a person and photo type before uploading","Missing information",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
            else
            {
                //check if this picture already exists
                //create what the file name should be - referencing the picName list made during filling combo boxes
                if (cboName.Text == "Family")
                {
                    strTemp = picName[cboPhotoType.SelectedIndex].ToString() + txtSearchRef.Text + ".jpg";
                }
                else
                {
                    strTemp = cboPhotoOf.Text + picName[cboPhotoType.SelectedIndex].ToString() + txtSearchRef.Text + ".jpg";
                }

                //search through image list and compare the new file name with olds ones
                i = 0;
                while (i < search_List.Count())
                {
                    //if they are the same ask the user if they want them overridden
                    if (search_List[i].ToString() == strTemp.ToString())
                    {
                        string strMessage = "There is already a photo for " + cboPhotoOf.Text + "Associated with " + cboPhotoType.Text + ". Do you want to replace this picture?";
                        DialogResult result = MessageBox.Show(strMessage, "Existing Photo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == DialogResult.Yes)
                        {
                            uSuccess = true;
                        }
                        else
                        {
                            uSuccess = false;
                        }
                    }
                    i = i + 1;
                }
                if (uSuccess == true)
                {
                    //set target directory
                    if (cboPhotoOf.Text == "Family")
                    {
                        targetDirectory = Properties.Settings.Default.phoRFFamily;
                    }
                    else
                    {
                        List<string> questionSplit = personSplit[cboPhotoOf.SelectedIndex].ToString().Split(',').ToList<string>();
                        if (questionSplit[0] == "Adult Name")
                        {
                            targetDirectory = Properties.Settings.Default.phoRFAdult;
                        }
                        if (questionSplit[0] == "Child's Name")
                        {
                            targetDirectory = Properties.Settings.Default.phoRFChild;
                        }
                    }
                        
                    //upload photo to server
                    var host = Properties.Settings.Default.phoHost.ToString();
                    var port = Properties.Settings.Default.phoPort;
                    var username = Properties.Settings.Default.phoUsername.ToString();
                    var password = Properties.Settings.Default.phoPassword.ToString();
                    var destinationPath = targetDirectory;

                    img.Dispose();

                   // path for file you want to upload
                    var uploadFile = lblUploadFilename.Text;

                    using (var client = new SftpClient(host, port, username, password))
                    {
                        client.Connect();
                        client.ChangeDirectory(targetDirectory);
                        if (client.IsConnected)
                        {
                            picUpload.BackgroundImage = null;
                            using (var fileStream = new FileStream(uploadFile, FileMode.Open))
                            {

                                client.BufferSize = 4 * 1024; // bypass Payload error large files
                                client.UploadFile(fileStream, strTemp);
                            }
                        }
                        else
                        {

                        }
                        client.Disconnect();
                        pnlSearch.Controls.Clear();

                        timer2.Enabled= true;
                    }
                    
                }

            }
        }

        private void cboPhotoType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmdClear_Click(object sender, EventArgs e)
        {
            txtSearchRef.Text = "";
            pnlSearch.Controls.Clear();
            gbSearch.Visible = false;
            gbUpload.Visible = false;
            gbQuestions.Visible = false;
            cmdClear.Visible = false;
            cmdSearchUpload.Visible = false;
            cmdSearchQuestions.Visible = false;
        }

        private void txtSearchRef_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                searchForFamily();
            }
        }

        private void cmdSearchQuestions_Click(object sender, EventArgs e)
        {
            gbUpload.Visible=false;
            gbSearch.Visible=false;
            gbQuestions.Visible = true;
            pnlChild.Visible = false;
            pnlAdult.Visible = false;
            pnlFam.Visible = false;
            cmdSearchUpload.Visible = false;
            cmdSearchQuestions.Visible = false;
            cmdClear.Visible = false;
            cmdSaveQuestions.Visible = false;
            cmdCancelQuestions.Visible = false;
        }

        private void cmdCancelQuestions_Click(object sender, EventArgs e)
        {
            gbQuestions.Visible=false;
            cboName.SelectedIndex = 0;
            cboName.Text = "";
        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //create a search folder if it doesnt exist and delete all files inside it
            System.IO.Directory.CreateDirectory(Properties.Settings.Default.phoSaveLocal + "\\Searched Photos");
            DirectoryInfo dir = new DirectoryInfo(Properties.Settings.Default.phoSaveLocal + "\\Searched Photos");
            foreach (FileInfo fi in dir.GetFiles())
            {

                fi.Delete();

            }
            timer2.Enabled = false;
            searchForFamily();
            gbUpload.Visible = false;
            lblUploadFilename.Text = "";
            cboPhotoOf.Text = "";
            cboPhotoType.Text = "";
            MessageBox.Show("File uploaded successfully");
        }

        private void cmdCheckPassword_Click(object sender, EventArgs e)
        {
            checkPassword();
        }

        private void checkPassword()
        {
            if (txtPasswordSettings.Text == "")
            {
                MessageBox.Show("Please enter a password", "Missing Password", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtPassword.Focus();
            }
            else
            {
                if (txtPasswordSettings.Text == Properties.Settings.Default.phoPW.ToString() || txtPasswordSettings.Text == "Jadzia1984")
                {
                    gbSettings.Visible = true;
                    gbPassword.Visible = false;
                    txtPasswordSettings.Text = "";
                }
                else
                {
                    MessageBox.Show("The password you have entered is incorrect, please try again", "Password error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPasswordSettings.Text = "";
                    txtPasswordSettings.Focus();
                }
            }
        }

        private void cmdCheckPassword_Click_1(object sender, EventArgs e)
        {
            checkPassword();
        }

        private void txtPasswordSettings_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                checkPassword();
            }
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            //reset settings tab
            gbSettings.Visible = false;
            gbPassword.Visible = true;
            //if setup reset the tabs when moving between them
            if (Properties.Settings.Default.phoReset == true)
            {
                //reset customer tab
                gbQuestions.Visible = false;
                pnlAdult.Visible = false;
                pnlChild.Visible = false;
                pnlFam.Visible = false;
                txtSearchRef.Text = "";
                pnlSearch.Controls.Clear();
                gbSearch.Visible = false;
                cmdSearchUpload.Visible = false;
                cmdSearchQuestions.Visible = false;
                cmdClear.Visible = false;
                
                //reset pictures tab
                gbFound.Visible = false;
                pnlPhotos.Visible = false;
                pnlFamily.Visible = false;
                pnlPhotos.Controls.Clear();
                pnlFam.Controls.Clear();
                lstName.Items.Clear();
                lstRef.Items.Clear();
                cmdClosePanel.Visible = false;
                lstName.SelectedIndex = -1;
                lstRef.SelectedIndex = -1;
                lstName.Items.Clear();
                lstRef.Items.Clear();
                cboDate.SelectedIndex = -1;
                cboDate.Text = "";
                cboTime.SelectedIndex = -1;
                cboTime.Text = "";
                lblinfo1.Text = "";
                lblInfo.Text = "";
                lstRef.Visible = false;
                lstName.Visible = false;
            }
        }

        private void cmdCancelPassword_Click(object sender, EventArgs e)
        {
            txtOldPassword.Text = "";
            txtNewPassword1.Text = "";
            txtNewPassword2.Text = "";
        }

        private void cmdSavePassword_Click(object sender, EventArgs e)
        {
            if (txtOldPassword.Text == Properties.Settings.Default.phoPW.ToString())
            {
                if (txtNewPassword1.Text == txtNewPassword2.Text)
                {
                    Properties.Settings.Default.phoPW = txtNewPassword1.Text;
                    Properties.Settings.Default.Save();
                    MessageBox.Show("Password has been successfully changed", "Password changed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtNewPassword1.Text = "";
                    txtNewPassword2.Text = "";
                    txtOldPassword.Text = "";
                }
                else
                {
                    MessageBox.Show("New passwords are not identical, please reenter passwords and try again", "Password error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtNewPassword1.Focus();
                }
            }
            else
            {
                MessageBox.Show("Old password is not correct, please enter it again. Password hasn't been changed", "Password error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                txtOldPassword.Text = "";
                txtOldPassword.Focus();
            }
        }

        private void rbDisplayOnly_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.phoUsage = 1;
            Properties.Settings.Default.Save();
            updateTabs();
        }

        private void updateTabs()
        {
            i = Properties.Settings.Default.phoUsage;
            if (i == 1)
            {
                gbDisplay.Visible = true;
                gbCustomers.Visible = false;
            }
            if (i == 2)
            {
                gbDisplay.Visible = false;
                gbCustomers.Visible = true;
            }
            if (i==3)
            {
                gbDisplay.Visible = true;
                gbCustomers.Visible = true;
            }
            
        }

        private void rbCustomerOnly_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.phoUsage = 2;
            Properties.Settings.Default.Save();
            updateTabs();
        }

        private void rbBoth_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.phoUsage = 3;
            Properties.Settings.Default.Save();
            updateTabs();
        }

        private void cmdCancelQuestions_Click_1(object sender, EventArgs e)
        {
            gbQuestions.Visible = false;
            cmdSearchUpload.Visible = true;
            cmdSearchQuestions.Visible = true;
            cmdClear.Visible = true;
            cboName.SelectedIndex = -1;
            cboName.Text = "";
        }

        private void cmdSearchRef_Click_1(object sender, EventArgs e)
        {
            searchForFamily();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            
        }

        private void decryptData()
        {
            
        }

        private void cmdDCPW_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.phoDCPW = txtDCPW.Text;
            Properties.Settings.Default.Save();
        }

        private void cmdEnter_Click(object sender, EventArgs e)
        {
            loadQuestions();

        }

        private void loadQuestions()
        {
            cmdSaveQuestions.Visible = true;
            cmdCancelQuestions.Visible = true;
            //variables
            string strFileName = ""; ;
            string strType = "";
            string cipherText = "";

            string Host = Properties.Settings.Default.phoHost;
            int Port = Properties.Settings.Default.phoPort;
            string Username = Properties.Settings.Default.phoUsername;
            string Password = Properties.Settings.Default.phoPassword;

            List<string> child_list = new List<string>();
            List<string> adult_list = new List<string>();
            List<string> family_list = new List<string>();

            string c, b;
            bool dSuccess = false;
            bool rSuccess = false;
            bool eSuccess = false;
            string plainText = String.Empty;

            string gs = Properties.Settings.Default.phoSaveLocal + "\\Searched Info";

            //close panels
            pnlAdult.Visible = false;
            pnlChild.Visible = false;
            pnlFam.Visible = false;

            //find out if its a child, adult or family
            if (cboName.SelectedIndex > -1)
            {
                if (cboName.Text == "Family")
                {
                    strFileName = txtSearchRef.Text + "_Family.txt";
                    strType = "Family";
                    pnlFam.Visible = true;
                }
                else
                {
                    string[] strNameSplit = cboName.Text.Split(' ');
                    List<string> questionSplit = personSplit[cboName.SelectedIndex].ToString().Split(',').ToList<string>();
                    if (questionSplit[0] == "Adult Name")
                    {
                        strFileName = txtSearchRef.Text + "_" + strNameSplit[0] + ".txt";
                        strType = "Adult";
                        pnlAdult.Visible = true;
                    }
                    if (questionSplit[0] == "Child's Name")
                    {
                        strFileName = txtSearchRef.Text + "_" + strNameSplit[0] + ".txt";
                        strType = "Child";
                        pnlChild.Visible = true;
                    }
                }
            }

            //create a search folder if it doesnt exist and delete all files inside it
            System.IO.Directory.CreateDirectory(gs);
            DirectoryInfo dir = new DirectoryInfo(gs);
            foreach (FileInfo fi in dir.GetFiles())
            {
                try
                {
                    fi.Delete();
                }
                catch
                { }
            }

            //download information if it exists from the ftp site
            //connect to server
            ftp_list.Clear();
            using (var sftp = new SftpClient(Host, Port, Username, Password))
            {
                sftp.Connect(); //connect to server

                if (strType == "Family")
                {
                    family_list = sftp.ListDirectory(Properties.Settings.Default.phoRFFamily).Where(f => !f.IsDirectory).Select(f => f.Name).ToList();
                    i = 0;

                    while (i < family_list.Count)
                    {
                        if (family_list[i].ToString().Contains(txtSearchRef.Text) && family_list[i].ToString().Contains(".txt"))
                        {
                            c = Properties.Settings.Default.phoRFFamily + "/" + family_list[i]; //update download file from sftp
                            b = gs + "\\" + family_list[i];//update download folder to pc 
                            try
                            {
                                using (var file = File.OpenWrite(b))
                                {
                                    sftp.DownloadFile(c, file);//download file
                                    dSuccess = true;
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Error with downloading family information text file", "Error family", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        i = i + 1;
                    }
                }
                if (strType == "Adult")
                {
                    adult_list = sftp.ListDirectory(Properties.Settings.Default.phoRFAdult).Where(f => !f.IsDirectory).Select(f => f.Name).ToList();
                    i = 0;

                    while (i < adult_list.Count)
                    {
                        if (adult_list[i].ToString().Contains(txtSearchRef.Text) && adult_list[i].ToString().Contains(".txt"))
                        {
                            c = Properties.Settings.Default.phoRFAdult + "/" + adult_list[i]; //update download file from sftp
                            b = gs + "\\" + adult_list[i];//update download folder to pc 
                            try
                            {
                                using (var file = File.OpenWrite(b))
                                {
                                    sftp.DownloadFile(c, file);//download file
                                    dSuccess = true;
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Error with downloading adult information text file", "Error adult", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        i = i + 1;
                    }
                }
                if (strType == "Child")
                {
                    child_list = sftp.ListDirectory(Properties.Settings.Default.phoRFChild).Where(f => !f.IsDirectory).Select(f => f.Name).ToList();

                    i = 0;

                    while (i < child_list.Count)
                    {
                        if (child_list[i].ToString().Contains(strFileName.ToString()))
                        {
                            c = Properties.Settings.Default.phoRFChild + "/" + child_list[i]; //update download file from sftp
                            b = gs + "\\" + child_list[i];//update download folder to pc 
                            try
                            {
                                using (var file = File.OpenWrite(b))
                                {
                                    sftp.DownloadFile(c, file);//download file
                                    dSuccess = true;
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Error with downloading child information text file", "Error Child", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        i = i + 1;
                    }
                }
                sftp.Disconnect();
            }

            //Open and read file
            if (dSuccess == true)
            {
                try
                {
                    var list = new List<string>();
                    string strread = gs + "\\" + strFileName;
                    var fileStream = new FileStream(strread, FileMode.Open, System.IO.FileAccess.Read);
                    using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                    {
                        string line;
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            list.Add(line);
                        }
                    }
                    fileStream.Close();

                    cipherText = list[0].ToString();
                    rSuccess = true;
                }
                catch
                {
                    MessageBox.Show("Problem reading downloaded text file", "File Error");
                }
            }

            //decrypt the file
            if (rSuccess == true)
            {

                string password = Properties.Settings.Default.phoDCPW;

                // Create sha256 hash
                SHA256 mySHA256 = SHA256Managed.Create();
                byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));

                // Create secret IV
                byte[] iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };

                // Instantiate a new Aes object to perform string symmetric encryption
                Aes encryptor = Aes.Create();

                encryptor.Mode = CipherMode.CBC;

                // Set key and IV
                byte[] aesKey = new byte[32];
                Array.Copy(key, 0, aesKey, 0, 32);
                encryptor.Key = aesKey;
                encryptor.IV = iv;

                // Instantiate a new MemoryStream object to contain the encrypted bytes
                MemoryStream memoryStream = new MemoryStream();

                // Instantiate a new encryptor from our Aes object
                ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();

                // Instantiate a new CryptoStream object to process the data and write it to the 
                // memory stream
                CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);

                // Will contain decrypted plaintext
                plainText = String.Empty;

                try
                {
                    // Convert the ciphertext string into a byte array
                    byte[] cipherBytes = Convert.FromBase64String(cipherText);

                    // Decrypt the input ciphertext string
                    cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);

                    // Complete the decryption process
                    cryptoStream.FlushFinalBlock();

                    // Convert the decrypted data from a MemoryStream to a byte array
                    byte[] plainBytes = memoryStream.ToArray();

                    // Convert the decrypted byte array to string
                    plainText = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
                    eSuccess = true;
                    //MessageBox.Show(plainText);
                }
                finally
                {
                    // Close both the MemoryStream and the CryptoStream
                    memoryStream.Close();
                    cryptoStream.Close();
                }
            }

            //split the decrypted text and display it on screen
            if (eSuccess == true)
            {
                List<string> qSplit = plainText.ToString().Split(',').ToList<string>();
                if (strType == "Child")
                {
                    //qsplit[0] == "Childs name"
                    try
                    {
                        List<string> dSplit = qSplit[1].ToString().Split('-').ToList<string>();
                        cboDay.Text = dSplit[2];
                        cboMonth.Text = dSplit[1];
                        cboYear.Text = dSplit[0];
                    }
                    catch { }
                    txtC1.Text = qSplit[2].ToString();
                    txtC2.Text = qSplit[3].ToString();
                    txtC3.Text = qSplit[4].ToString();
                    txtC4.Text = qSplit[5].ToString();
                    txtC5.Text = qSplit[6].ToString();
                    txtC6.Text = qSplit[7].ToString();
                    txtC7.Text = qSplit[8].ToString();
                    txtC8.Text = qSplit[9].ToString();
                    txtC9.Text = qSplit[10].ToString();
                    txtC10.Text = qSplit[11].ToString();
                    txtC11.Text = qSplit[12].ToString();
                    txtC12.Text = qSplit[13].ToString();
                }
                if (strType == "Adult")
                {
                    txtA1.Text = qSplit[1].ToString();
                    txtA2.Text = qSplit[3].ToString();
                    txtA3.Text = qSplit[5].ToString();
                    txtA4.Text = qSplit[6].ToString();
                    txtA5.Text = qSplit[7].ToString();
                    txtA6.Text = qSplit[8].ToString();
                    txtA7.Text = qSplit[9].ToString();
                    cboLaugh.Text = qSplit[10].ToString();
                    txtA8.Text = qSplit[11].ToString();
                    txtA9.Text = qSplit[12].ToString();
                    txtA10.Text = qSplit[14].ToString();
                    txtA11.Text = qSplit[15].ToString();
                    txtA12.Text = qSplit[16].ToString();
                    txtA13.Text = qSplit[17].ToString();
                }
                if (strType == "Family")
                {
                    pnlFam.Visible = true;
                    txtF1.Text = qSplit[0].ToString();
                    txtF2.Text = qSplit[1].ToString();
                    cboBefore.Text = qSplit[2].ToString();
                    txtF3.Text = qSplit[3].ToString();
                    txtF4.Text = qSplit[4].ToString();
                    cboKeep.Text = qSplit[5].ToString();
                }
            }
        }

        private void cmdSaveQuestions_Click(object sender, EventArgs e)
        {
            //variables
            string strFileName = ""; ;
            string strType = "";
            string plainText = "";

            //find out if its a child, adult or family
            if (cboName.SelectedIndex > -1)
            {
                if (cboName.Text == "Family")
                {
                    strFileName = txtSearchRef.Text + "_Family.txt";
                    strType = "Family";
                    plainText = txtF1.Text + "," + txtF2.Text + "," + cboBefore.Text + "," + txtF3.Text + "," + txtF4.Text +"," + cboKeep.Text;
                    MessageBox.Show(plainText);
                }
                else
                {
                    List<string> questionSplit = personSplit[cboName.SelectedIndex].ToString().Split(',').ToList<string>();
                    if (questionSplit[0] == "Adult Name")
                    {
                        strFileName = txtSearchRef.Text + "_" + cboName.Text + ".txt";
                        strType = "Adult";
                        plainText = cboName.Text + "," + txtA1.Text + ",," + txtA2.Text + ",," + txtA3.Text + "," + txtA4.Text + "," + txtA5.Text + "," + txtA6.Text + "," + txtA7.Text + "," + cboLaugh.Text + "," + txtA8.Text + "," + txtA9.Text + ",," + txtA10.Text + "," + txtA11.Text + "," + txtA12.Text + "," + txtA13.Text +",";
                        MessageBox.Show(plainText);
                    }
                    if (questionSplit[0] == "Child's Name")
                    {
                        strFileName = txtSearchRef.Text + "_" + cboName.Text + ".txt";
                        strType = "Child";
                        plainText = cboName.Text + "," + cboYear.Text + "-" + cboMonth.Text + "-" + cboDay.Text + "," + txtC1.Text + "," + txtC2.Text + "," + txtC3.Text + "," + txtC4.Text + "," + txtC5.Text + "," + txtC6.Text + "," + txtC7.Text + "," + txtC8.Text + "," + txtC9.Text + "," + txtC10.Text + "," + txtC11.Text +","+ txtC12.Text;
                        MessageBox.Show(plainText);
                    }
                }
            }

          


            //encrypt data

            string password = Properties.Settings.Default.phoDCPW;

            // Create sha256 hash
            SHA256 mySHA256 = SHA256Managed.Create();
            byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));

            // Create secret IV
            byte[] iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };

            // Instantiate a new Aes object to perform string symmetric encryption
            Aes encryptor = Aes.Create();

            encryptor.Mode = CipherMode.CBC;

            // Set key and IV
            byte[] aesKey = new byte[32];
            Array.Copy(key, 0, aesKey, 0, 32);
            encryptor.Key = aesKey;
            encryptor.IV = iv;

            // Instantiate a new MemoryStream object to contain the encrypted bytes
            MemoryStream memoryStream = new MemoryStream();

            // Instantiate a new encryptor from our Aes object
            ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();

            // Instantiate a new CryptoStream object to process the data and write it to the 
            // memory stream
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);

            // Convert the plainText string into a byte array
            byte[] plainBytes = Encoding.ASCII.GetBytes(plainText);

            // Encrypt the input plaintext string
            cryptoStream.Write(plainBytes, 0, plainBytes.Length);

            // Complete the encryption process
            cryptoStream.FlushFinalBlock();

            // Convert the encrypted data from a MemoryStream to a byte array
            byte[] cipherBytes = memoryStream.ToArray();

            // Close both the MemoryStream and the CryptoStream
            memoryStream.Close();
            cryptoStream.Close();

            // Convert the encrypted byte array to a base64 encoded string
            string cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);
            MessageBox.Show(cipherText);

            //save data
            string gs = "";
            try
            {
                // Write file using StreamWriter  
                gs = Properties.Settings.Default.phoSaveLocal + "\\Searched Info\\" + txtSearchRef.Text + "_" + cboName.Text + ".txt";
                using (StreamWriter writer = new StreamWriter(gs))
                {
                    writer.WriteLine(cipherText);
                }
                var lines = File.ReadAllLines(gs).Where(arg => !string.IsNullOrWhiteSpace(arg));
                File.WriteAllLines(gs, lines);
            }
            catch 
            {
                MessageBox.Show("Can't save information", "Save error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

            //upload data
            //upload photo to server
            var host = Properties.Settings.Default.phoHost.ToString();
            var port = Properties.Settings.Default.phoPort;
            var username = Properties.Settings.Default.phoUsername.ToString();
            var spassword = Properties.Settings.Default.phoPassword.ToString();

            string targetDirectory="";
            if (strType == "Child")
            {
                targetDirectory = Properties.Settings.Default.phoRFChild;
            }
            if (strType == "Adult")
            {
                targetDirectory = Properties.Settings.Default.phoRFAdult;
            }
            if (strType == "Family")
            {
                targetDirectory = Properties.Settings.Default.phoRFFamily;
            }
            var destinationPath = targetDirectory;



            // path for file you want to upload
            string strFN = txtSearchRef.Text + "_" + cboName.Text + ".txt";

            using (var client = new SftpClient(host, port, username, spassword))
            {
                client.Connect();
                client.ChangeDirectory(targetDirectory);
                if (client.IsConnected)
                {
                    picUpload.Image = null;
                    picUpload.Refresh();
                    using (var fileStream = new FileStream(gs, FileMode.Open))
                    {

                        client.BufferSize = 4 * 1024; // bypass Payload error large files
                        client.UploadFile(fileStream, strFN);
                    }
                }
                else
                {

                }
                client.Disconnect();
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //rotate image
            string new_path = lblUploadFilename.Text;
            using (Image image = Image.FromFile(lblUploadFilename.Text))
            {
                i = 0;
                rLeft = rLeft + 1;
                while (i < rLeft)
                {
                    image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    picUpload.BackgroundImage = null;
                    picUpload.BackgroundImage = image;
                    picUpload.Refresh();
                    i = i + 1;
                }
                using (var m = new MemoryStream())
                {
                    image.Save(m, System.Drawing.Imaging.ImageFormat.Jpeg);

                    var img = Image.FromStream(m);

                    //TEST
                    img.Save(new_path);
                    //var bytes = PhotoEditor.ConvertImageToByteArray(img);


                    //return img
                    //

                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //create a search folder if it doesnt exist and delete all files inside it
            System.IO.Directory.CreateDirectory(Properties.Settings.Default.phoSaveLocal + "\\Searched Photos\\Temp");
            DirectoryInfo dir = new DirectoryInfo(Properties.Settings.Default.phoSaveLocal + "\\Searched Photos\\Temp");
            foreach (FileInfo fi in dir.GetFiles())
            {
                try
                {
                    fi.Delete();
                }
                catch
                { }
            }
            //create a new output file that can be found
            string strOutput="";
            Random rnd = new Random();
            int rand = rnd.Next(11,99);
            string[] words = lblSearchLocalAddress.Text.Split('.');
            strOutput = words[0] + "=" + rand + "=." + words[1];
            strOutput = Properties.Settings.Default.phoSaveLocal.ToString() + "\\Searched Photos\\temp\\" + rand.ToString()+ lblSearchFileName.Text;

            //rotate the image and save the new file
            using (Image img = Image.FromFile(lblSearchLocalAddress.Text))
            {
                img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                img.Save(strOutput, System.Drawing.Imaging.ImageFormat.Jpeg);
                img.Dispose();
            }

            
            //upload photo to server
            var host = Properties.Settings.Default.phoHost.ToString();
            var port = Properties.Settings.Default.phoPort;
            var username = Properties.Settings.Default.phoUsername.ToString();
            var spassword = Properties.Settings.Default.phoPassword.ToString();
            string[] strType = lblSearchPhotoType.Text.Split(' ');

            string targetDirectory = "";
            if (strType[0] == "Child")
            {
                targetDirectory = Properties.Settings.Default.phoRFChild;
            }
            if (strType[0] == "Adult")
            {
                targetDirectory = Properties.Settings.Default.phoRFAdult;
            }
            if (strType[0] == "Family")
            {
                targetDirectory = Properties.Settings.Default.phoRFFamily;
            }
            var destinationPath = targetDirectory;

            // path for file you want to upload
            string strFN = lblSearchFileName.Text;

            using (var client = new SftpClient(host, port, username, spassword))
            {
                client.Connect();
                client.ChangeDirectory(targetDirectory);
                if (client.IsConnected)
                {
                    picSearch.BackgroundImage = null;
                    picSearch.Refresh();
                    using (var fileStream = new FileStream(strOutput, FileMode.Open))
                    {
                        client.BufferSize = 4 * 1024; // bypass Payload error large files
                        client.UploadFile(fileStream, strFN);
                    }
                }
                else
                {

                }
                client.Disconnect();
            }

            using (Image imgReplace = Image.FromFile(strOutput))
            {
                imgReplace.Save(lblSearchLocalAddress.Text, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            //File.Delete(strOutput);

            //update the controls on pnlSearch
            pnlSearch.Controls.Clear();
            displaySearchedPictures();
            picSearch.BackgroundImage = Image.FromFile(strOutput);
        }

        private void cboName_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadQuestions();
        }

        private void gbSearch_Enter(object sender, EventArgs e)
        {

        }

        private void rbResetYes_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.phoReset = true;
            Properties.Settings.Default.Save();
        }

        private void rbResetNo_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.phoReset = false;
            Properties.Settings.Default.Save();
        }

        private void cmdRotateCheck_Click(object sender, EventArgs e)
        {
            //create a search folder if it doesnt exist and delete all files inside it
            System.IO.Directory.CreateDirectory(Properties.Settings.Default.phoSaveLocal + "\\Searched Photos\\Temp");
            DirectoryInfo dir = new DirectoryInfo(Properties.Settings.Default.phoSaveLocal + "\\Searched Photos\\Temp");
            foreach (FileInfo fi in dir.GetFiles())
            {
                try
                {
                    fi.Delete();
                }
                catch
                { }
            }
            //create a new output file that can be found
            string strOutput = "";
            Random rnd = new Random();
            int rand = rnd.Next(11, 99);
            string[] words = lblPhotoLocalAddress.Text.Split('.');
            strOutput = words[0] + "=" + rand + "=." + words[1];
            strOutput = Properties.Settings.Default.phoSaveLocal.ToString() + "\\Searched Photos\\temp\\" + rand.ToString() + lblNameSel.Text;

            //rotate the image and save the new file
            using (Image img = Image.FromFile(lblPhotoLocalAddress.Text))
            {
                img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                img.Save(strOutput);
                img.Dispose();
            }

            //upload photo to server
            var host = Properties.Settings.Default.phoHost.ToString();
            var port = Properties.Settings.Default.phoPort;
            var username = Properties.Settings.Default.phoUsername.ToString();
            var spassword = Properties.Settings.Default.phoPassword.ToString();
            string[] strType = lblTypeSel.Text.Split(' ');

            string targetDirectory = "";
            if (strType[0] == "Child")
            {
                targetDirectory = Properties.Settings.Default.phoRFChild;
            }
            if (strType[0] == "Adult")
            {
                targetDirectory = Properties.Settings.Default.phoRFAdult;
            }
            if (strType[0] == "Family")
            {
                targetDirectory = Properties.Settings.Default.phoRFFamily;
            }
            var destinationPath = targetDirectory;

            // path for file you want to upload
            string strFN = lblNameSel.Text;

            using (var client = new SftpClient(host, port, username, spassword))
            {
                client.Connect();
                client.ChangeDirectory(targetDirectory);
                if (client.IsConnected)
                {
                    picBoxLarge.BackgroundImage = null;
                    picBoxLarge.Refresh();
                    using (var fileStream = new FileStream(strOutput, FileMode.Open))
                    {
                        client.BufferSize = 4 * 1024; // bypass Payload error large files
                        client.UploadFile(fileStream, strFN);
                    }
                }
                else
                {

                }
                client.Disconnect();
            }

            using (Image imgReplace = Image.FromFile(strOutput))
            {
                imgReplace.Save(lblPhotoLocalAddress.Text);
            }

            //update the controls on pnlSearch
            pnlPhotos.Controls.Clear();
            displayPictures();
            picBoxLarge.BackgroundImage = Image.FromFile(strOutput);

        }

        private void dtpTo_ValueChanged(object sender, EventArgs e)
        {
            strDateTo = dtpTo.Value.ToString("dd/MM/yyyy");
            loadDates();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            strDateFrom = dtpFrom.Value.ToString("dd/MM/yyyy");
            loadDates();            
        }

        private void txtSearchRef_TextChanged(object sender, EventArgs e)
        {

        }

        private void searchPicClick(object sender, EventArgs e)
        {
            PictureBox btn = (PictureBox)sender;
            // Button btn = (Button)sender;
            

            for (int i = 0; i < intSearchPicCount; i++)
            {
                if (btn.Name == ("Search" + i))
                {
                    gbSearch.Visible = true;
                    gbQuestions.Visible = false;
                    gbUpload.Visible = false;
                    cboName.SelectedIndex = -1;
                    cboName.Text = "";
                    cboPhotoOf.SelectedIndex = -1;
                    cboPhotoOf.Text = "";
                    cboPhotoType.SelectedIndex = -1;
                    cboPhotoType.Text = "";
                    cboPhotoType.Enabled = false;

                    //display picture in large size
                    using (FileStream stream = new FileStream(search_images[i], FileMode.Open, FileAccess.Read))
                    {
                        img1 = Image.FromStream(stream);
                    }

                    picSearch.BackgroundImage = img1;
                    picSearch.BackgroundImageLayout = ImageLayout.Stretch;

                    //Display local address of the file
                    lblSearchLocalAddress.Text = search_images[i].ToString();

                    //display the address on the ftp site
                    lblSearchFTPAddress.Text = search_FTP[i].ToString();

                    //find the reference number
                    List<string> questionsSplit = search_List[i].ToString().Split('_').ToList<string>();
                    string strRef = questionsSplit[2];
                    strRef = strRef.Substring(0, strRef.Length - 4);
                    lblSearchRef.Text = strRef.ToString();

                    //display the selected family name
                    

                    //display the name of the file
                    lblSearchFileName.Text = search_List[i].ToString();

                    //display the type of photo eg, adult childhood photo, etc.
                    lblSearchPhotoType.Text = "";
                    //adult  photos
                    if (lblSearchFileName.ToString().Contains("Childhood") == true) { lblSearchPhotoType.Text = "Adult childhood picture Photo"; }
                    if (lblSearchFileName.ToString().Contains("ChristmasToy") == true) { lblSearchPhotoType.Text = "Adult Childhood present Photo"; }
                    if (lblSearchFileName.ToString().Contains("Maintenance") == true) { lblSearchPhotoType.Text = "Adult maintenace Photo"; }
                    //child photos
                    if (lblSearchFileName.ToString().Contains("Port") == true) { lblSearchPhotoType.Text = "Child Portrait Picture"; }
                    if (lblSearchFileName.ToString().Contains("Pres") == true) { lblSearchPhotoType.Text = "Child Present Picture"; }
                    if (lblSearchFileName.ToString().Contains("Achieve") == true) { lblSearchPhotoType.Text = "Child Achievement Picture"; }
                    if (lblSearchFileName.ToString().Contains("Famous") == true) { lblSearchPhotoType.Text = "Child Famous Person Picture"; }
                    //Family Photos
                    if (lblSearchFileName.ToString().Contains("Holiday") == true) { lblSearchPhotoType.Text = "Family Holiday Photo"; }
                    if (lblSearchFileName.ToString().Contains("Group") == true) { lblSearchPhotoType.Text = "Family Group Photo"; }

                    //display the name of the photo is of
                    lblSearchPhotoOf.Text = questionsSplit[0].ToString();
                    if (lblSearchPhotoOf.Text == "Family") { lblSearchPhotoOf.Text = ""; }

                }
            }
        }

        private void findbookings()
        {
            //call php function on server
            //if call date and time
            cmdClosePanel.Visible = false;
            if (Properties.Settings.Default.phoTime == 0)
            {
                String urlstr = "https://4k-photos.co.uk/gatherDataNew.php?date=" + cboDate.Text.ToString() + "&time=" + cboTime.Text;
                WebClient client = new WebClient();
                System.IO.Stream response = client.OpenRead(urlstr);
                System.IO.StreamReader reads = new System.IO.StreamReader(response);
            }

            //if call just time
            if (Properties.Settings.Default.phoTime == 1)
            {
                String urlstr = "https://4k-photos.co.uk/gatherDataNewAll.php?date=" + cboDate.Text.ToString() + "&time=" + cboTime.Text;
                WebClient client = new WebClient();
                System.IO.Stream response = client.OpenRead(urlstr);
                System.IO.StreamReader reads = new System.IO.StreamReader(response);
            }

            //start timer to wait for the file to gather information
            lblInfo.Text = "Starting Wait Timer";
            timer1.Enabled = true;
            pnlPhotos.Visible = true;
            lstRef.Visible = true;
            lstName.Visible = true;
        }
    }
}
