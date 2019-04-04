// Program 2
// CIS 200-01/76
// Fall 2017
// Due: 10/23/2017
// By: Andrew L. Wright (Students use Grading ID)

// File: Prog2Form.cs
// This class creates the main GUI for Program 2. It provides a
// File menu with About and Exit items, an Insert menu with Address and
// Letter items, and a Report menu with List Addresses and List Parcels
// items.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Prog2;

namespace UPVApp

{
    public partial class Prog2Form : Form
    {
        private UserParcelView upv; // The UserParcelView
        private IFormatter formattter = new BinaryFormatter(); //Used for outputting Addresses
        private FileStream stream; //Used for outputting addresses
        private BinaryFormatter reader = new BinaryFormatter(); //Used for reading addresses
        private FileStream input;// Used for reading addresses


        // Precondition:  None
        // Postcondition: The form's GUI is prepared for display. A few test addresses are
        //                added to the list of addresses
        public Prog2Form()
        {
            InitializeComponent();

            upv = new UserParcelView();          
        }

        // Precondition:  File, About menu item activated
        // Postcondition: Information about author displayed in dialog box
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string NL = Environment.NewLine; // Newline shorthand

            MessageBox.Show($"Program 2{NL}By: Andrew L. Wright{NL}CIS 200{NL}Fall 2017",
                "About Program 2");
        }

        // Precondition:  File, Exit menu item activated
        // Postcondition: The application is exited
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Precondition:  Insert, Address menu item activated
        // Postcondition: The Address dialog box is displayed. If data entered
        //                are OK, an Address is created and added to the list
        //                of addresses
        private void addressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddressForm addressForm = new AddressForm();    // The address dialog box form
            DialogResult result = addressForm.ShowDialog(); // Show form as dialog and store result
            int zip; // Address zip code

            if (result == DialogResult.OK) // Only add if OK
            {
                if (int.TryParse(addressForm.ZipText, out zip))
                {
                    upv.AddAddress(addressForm.AddressName, addressForm.Address1,
                        addressForm.Address2, addressForm.City, addressForm.State,
                        zip); // Use form's properties to create address
                }
                else // This should never happen if form validation works!
                {
                    MessageBox.Show("Problem with Address Validation!", "Validation Error");
                }
            }

            addressForm.Dispose(); // Best practice for dialog boxes
                                   // Alternatively, use with using clause as in Ch. 17
        }

        // Precondition:  Report, List Addresses menu item activated
        // Postcondition: The list of addresses is displayed in the addressResultsTxt
        //                text box
        private void listAddressesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder result = new StringBuilder(); // Holds text as report being built
                                                        // StringBuilder more efficient than String
            string NL = Environment.NewLine;            // Newline shorthand

            result.Append("Addresses:");
            result.Append(NL); // Remember, \n doesn't always work in GUIs
            result.Append(NL);

            foreach (Address a in upv.AddressList)
            {
                result.Append(a.ToString());
                result.Append(NL);
                result.Append("------------------------------");
                result.Append(NL);
            }

            reportTxt.Text = result.ToString();

            // -- OR --
            // Not using StringBuilder, just use TextBox directly

            //reportTxt.Clear();
            //reportTxt.AppendText("Addresses:");
            //reportTxt.AppendText(NL); // Remember, \n doesn't always work in GUIs
            //reportTxt.AppendText(NL);

            //foreach (Address a in upv.AddressList)
            //{
            //    reportTxt.AppendText(a.ToString());
            //    reportTxt.AppendText(NL);
            //    reportTxt.AppendText("------------------------------");
            //    reportTxt.AppendText(NL);
            //}

            // Put cursor at start of report
            reportTxt.Focus();
            reportTxt.SelectionStart = 0;
            reportTxt.SelectionLength = 0;
        }

        // Precondition:  Insert, Letter menu item activated
        // Postcondition: The Letter dialog box is displayed. If data entered
        //                are OK, a Letter is created and added to the list
        //                of parcels
        private void letterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LetterForm letterForm; // The letter dialog box form
            DialogResult result;   // The result of showing form as dialog
            decimal fixedCost;     // The letter's cost

            if (upv.AddressCount < LetterForm.MIN_ADDRESSES) // Make sure we have enough addresses
            {
                MessageBox.Show("Need " + LetterForm.MIN_ADDRESSES + " addresses to create letter!",
                    "Addresses Error");
                return; // Exit now since can't create valid letter
            }

            letterForm = new LetterForm(upv.AddressList); // Send list of addresses
            result = letterForm.ShowDialog();

            if (result == DialogResult.OK) // Only add if OK
            {
                if (decimal.TryParse(letterForm.FixedCostText, out fixedCost))
                {
                    // For this to work, LetterForm's combo boxes need to be in same
                    // order as upv's AddressList
                    upv.AddLetter(upv.AddressAt(letterForm.OriginAddressIndex),
                        upv.AddressAt(letterForm.DestinationAddressIndex),
                        fixedCost); // Letter to be inserted
                }
               else // This should never happen if form validation works!
                {
                    MessageBox.Show("Problem with Letter Validation!", "Validation Error");
                }
            }

            letterForm.Dispose(); // Best practice for dialog boxes
                                  // Alternatively, use with using clause as in Ch. 17
        }

        // Precondition:  Report, List Parcels menu item activated
        // Postcondition: The list of parcels is displayed in the parcelResultsTxt
        //                text box
        private void listParcelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder result = new StringBuilder(); // Holds text as report being built
                                                        // StringBuilder more efficient than String
            decimal totalCost = 0;                      // Running total of parcel shipping costs
            string NL = Environment.NewLine;            // Newline shorthand

            result.Append("Parcels:");
            result.Append(NL); // Remember, \n doesn't always work in GUIs
            result.Append(NL);

            foreach (Parcel p in upv.ParcelList)
            {
                result.Append(p.ToString());
                result.Append(NL);
                result.Append("------------------------------");
                result.Append(NL);
                totalCost += p.CalcCost();
            }

            result.Append(NL);
            result.Append($"Total Cost: {totalCost:C}");

            reportTxt.Text = result.ToString();

            // Put cursor at start of report
            reportTxt.Focus();
            reportTxt.SelectionStart = 0;
            reportTxt.SelectionLength = 0;
        }

        //Precondition: user clicks on open in the menu
        //Postconditon: slected file has its data imported into the program and it is used to populate the address list
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result;
            string fileName;

            using (OpenFileDialog fileOpener = new OpenFileDialog())
            {
                result = fileOpener.ShowDialog();
                fileName = fileOpener.FileName;
            }

            if (result == DialogResult.OK)
            {
                if (fileName == string.Empty)
                {
                    MessageBox.Show("Invalid file name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    //addressList.Clear();
                    //parcelList.Clear();
                    //upv.AddressList.Clear();
                    input = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                    List<Address> inputAddressList = (List<Address>)formattter.Deserialize(input);
                    foreach (Address address in inputAddressList)
                    {
                        upv.AddAddress(address.Name, address.Address1, address.Address2, address.City, address.State, address.Zip);                       
                    }
                }

            }
        }

        //Precondition: user clicks on save in the menu
        //Postcondition: current address list is saved and exported into a file
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result;
            string fileName;

            using (SaveFileDialog fileChooser = new SaveFileDialog())
            {
                fileChooser.CheckFileExists = false;

                result = fileChooser.ShowDialog();
                fileName = fileChooser.FileName;
            }

            if (result == DialogResult.OK)
            {
                if (fileName == string.Empty)

                    MessageBox.Show("Invalid File Name", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    try
                    {
                        stream = new FileStream(fileName,
                            FileMode.Create, FileAccess.Write);

                        formattter.Serialize(stream, upv.AddressList);
                        stream.Close();
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("Error Saving file.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        //Precondition: user clicks on addresses under the edit menu.
        //Postcondition: new window will appear allowing user to edit address.
        private void addressToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            EditAddressForm editAddress = new EditAddressForm(upv.addresses);// send addresses to new form
            DialogResult result = editAddress.ShowDialog(); // The result of showing form as dialog
            int addressIndex; //used to hold the position of the addresses

            if (result == DialogResult.OK)
            {
                addressIndex = editAddress.index; //inherit the selected address index
                AddressForm editAdress = new AddressForm(); //Create a new Address form

                //fills text boxes with information from the addresses 
                editAdress.Address1 = upv.addresses[addressIndex].Address1;
                editAdress.Address2 = upv.addresses[addressIndex].Address2;
                editAdress.AddressName = upv.addresses[addressIndex].Name;
                editAdress.City = upv.addresses[addressIndex].City;
                editAdress.State = upv.addresses[addressIndex].State;
                editAdress.ZipText = upv.addresses[addressIndex].Zip.ToString();

                DialogResult editResult = editAdress.ShowDialog();//Stores the dialog result from editAddress

                if (editResult == DialogResult.OK)
                {
                    //puts the information back into the address list that the user edited.
                    upv.addresses[addressIndex].Address1 = editAdress.Address1;
                    upv.addresses[addressIndex].Address2 = editAdress.Address2;
                    upv.addresses[addressIndex].Name = editAdress.AddressName;
                    upv.addresses[addressIndex].City = editAdress.City;
                    upv.addresses[addressIndex].State = editAdress.State;
                    upv.addresses[addressIndex].Zip = Convert.ToInt32(editAdress.ZipText);

                }
            }
        }
    }
}