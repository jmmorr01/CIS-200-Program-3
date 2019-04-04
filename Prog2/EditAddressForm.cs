// Grading ID C2937
// Program 3
//Section CIS 200-01
// Due: November 13th, 2017
// Description: this form allows the user to choose an address and open it, allowing the user to edit the information related to the address.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prog2
{
    public partial class EditAddressForm : Form
    {
        List<Address> addressList; //used to import the addressList in the constructor
        ErrorProvider errorprovider = new ErrorProvider(); //ErrorProvider
        public int index; // Index used to validate the chosen index

        //Precondition: address list is populated with the available addresses
        //Postcondition: the form is displayed
        public EditAddressForm(List<Address> addresses)
        {
            addressList = addresses;
            InitializeComponent();

            foreach (Address var in addressList)
            {
                addressChoice.Items.Add(var.Name.ToString());
            }
        }

        //Precondition: user presses the cancel button
        //Postcondition: the form is closed.
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Precondition: user clicks on the ok button
        //Postcondition: all choices are validated and new address form is opened if no error is provided. 
        private void okButton_Click(object sender, EventArgs e)
        {
            if (ValidateChildren())
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        //Precondition: focus shifts from one of the addresses in the combobox
        //Postcondition: Error is provided if no valid address is selected
        private void addressChoice_Validating(object sender, CancelEventArgs e)
        {
            if (addressChoice.SelectedIndex == -1)
            {
                e.Cancel = true;
                errorprovider.SetError(addressChoice, "Please select a valid address");
            }
        }

        //Precondition: validating of sender is not cancelled
        //Postcondition: error provider is cleared
        private void addressChoice_Validated(object sender, EventArgs e)
        {
            errorprovider.SetError(addressChoice, "");
        }

        //Precondition: user selects an item in the combobox
        //Postcondition: the variable has its value changed to reflect the new selection 
        private void addressChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            index = addressChoice.SelectedIndex;
        }
    }
}
