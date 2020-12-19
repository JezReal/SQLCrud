using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace SQLCrud
{
    public partial class Form1 : Form
    {
        //DATABASE STRING SYNTAX, PASSWORD MAY BE LEFT EMPTY OR NOT CODED ENTIRELY
        private const string ConnectionString = @"Server=localhost;Database=studentdb;Uid=root;";
        int _studentId = 0;

        private bool _isOperationUpdate;
        private bool valid=true;
        
        public Form1()
        {
            InitializeComponent();
        }


        //09-Nov-2019 BSIT 2A/2B
        //ADDING OF RECORDS
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (fieldsIsValid()) {
                using (MySqlConnection mySqlConnection = new MySqlConnection(ConnectionString))
                {
                    try
                    {
                        mySqlConnection.Open();
                        MySqlCommand mySqlCmd = new MySqlCommand("StudentAddOrEdit", mySqlConnection);
                        mySqlCmd.CommandType = CommandType.StoredProcedure;
                        mySqlCmd.Parameters.AddWithValue("_StudentID", _studentId);
                        mySqlCmd.Parameters.AddWithValue("_StudentName", txtStudentName.Text.Trim());
                        mySqlCmd.Parameters.AddWithValue("_StudentAddress", txtAddress.Text.Trim());
                        mySqlCmd.Parameters.AddWithValue("_Description", txtDescription.Text.Trim());
                        mySqlCmd.ExecuteNonQuery();

                        Clear();
                        GridFill();

                        if (_isOperationUpdate)
                        {
                            MessageBox.Show("Student has been updated");
                        }
                        else
                        {
                            MessageBox.Show("Student has been added");
                        }

                        _isOperationUpdate = false;

                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Incomplete fields.");
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            TestConnection();
            Clear(); //CLEARS RECORD TO GIVE WAY FOR THE POPULATION OF FRESH RECORDS (GRIDFILL)
            GridFill(); //POPULATES THE DATA GRID VIEW OF RECORDS FROM OUR DATABASE
        }


        //POPULATING OF DATA GRID VIEW WITH DATABASE RECORDS
        private void GridFill()
        {
            using (MySqlConnection mySqlConnection = new MySqlConnection(ConnectionString))
            {
                mySqlConnection.Open();
                MySqlDataAdapter sqlDa = new MySqlDataAdapter("StudentViewAll", mySqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dtblStudent = new DataTable();
                sqlDa.Fill(dtblStudent);
                dgvStudent.DataSource = dtblStudent;
                dgvStudent.Columns[0].Visible = false;
            }
        }

        //CLEARS ALL INPUT FIELDS / TEXT BOXES USED INSIDE THE FORM
        private void Clear()
        {
            txtStudentName.Text = txtAddress.Text = txtDescription.Text = txtSearch.Text = "";
            _studentId = 0;
            btnSave.Text = "Save";
            btnDelete.Enabled = false;
        }

        //ALLOWS SELECTION OF RECORDS INSIDE THE DATA GRID VIEW
        private void DgvStudent_DoubleClick(object sender, EventArgs e)
        {
            if (dgvStudent.CurrentRow.Index != -1)
            {
                txtStudentName.Text = dgvStudent.CurrentRow.Cells[1].Value.ToString();
                txtAddress.Text = dgvStudent.CurrentRow.Cells[2].Value.ToString();
                txtDescription.Text = dgvStudent.CurrentRow.Cells[3].Value.ToString();
                _studentId = Convert.ToInt32(dgvStudent.CurrentRow.Cells[0].Value.ToString());
                btnSave.Text = "Update";
                btnDelete.Enabled = Enabled;
                _isOperationUpdate = true;
            }
        }

        //ADDING OF SEARCH FILTER TO OUR DATA GRID VIEW FOR FASTER ACCESS TO A SPECIFIC RECORD 
        // AND TO SIMULATE ANOTHER READING OPERATION ON SQL (PLEASE REFER TO THE ROUTINE)
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            DisplayItems();
        }

        //CLICKING THE CANCEL BUTTON TRIGGERS OUR CLEAR METHOD
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        //PLEASE REFER TO OUR ROUTINE, THIS DELETES THE RECORD CURRENTLY SELECTED
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            using (MySqlConnection mySqlConnection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    mySqlConnection.Open();
                    MySqlCommand mySqlCmd = new MySqlCommand("StudentDeleteByID", mySqlConnection);
                    mySqlCmd.CommandType = CommandType.StoredProcedure;
                    mySqlCmd.Parameters.AddWithValue("_StudentID", _studentId);
                    mySqlCmd.ExecuteNonQuery();
                    MessageBox.Show("Deleted Successfully");
                    Clear();
                    GridFill();

                } catch(Exception sqlException) 
                {
                    MessageBox.Show(sqlException.Message);
                } 
            }
        }

        private static void TestConnection()
        {
            using (MySqlConnection mySqlConnection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    mySqlConnection.Open();
                }
                catch (MySqlException)
                {
                    MessageBox.Show("It seems the database is offline. Please turn it on then rerun the program.",
                        "Database Offline");

                    //use environment.exit instead of application.exit() because at this point the Application.Run() has not been called yet
                    Environment.Exit(-1);
                }
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            // fill the table when the search box does not contain any text
            // user no longer has to press the search button to fill the table
            if (txtSearch.Text.Length == 0)
            {
                DisplayItems();
            }
            else
            {
                return;
            }
        }

        private void DisplayItems()
        {
            using (MySqlConnection mySqlConnection = new MySqlConnection(ConnectionString))
            {
                mySqlConnection.Open();
                MySqlDataAdapter sqlDa = new MySqlDataAdapter("StudentSearchByValue", mySqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("_SearchValue", txtSearch.Text);
                DataTable dtblStudent = new DataTable();
                sqlDa.Fill(dtblStudent);
                dgvStudent.DataSource = dtblStudent;
                dgvStudent.Columns[0].Visible = false;
            }
        }

        private bool fieldsIsValid()
        {
            List<TextBox> fields = new List<TextBox>();
            fields.Add(txtDescription);
            fields.Add(txtAddress);
            fields.Add(txtStudentName);

            foreach (TextBox field in fields)
            {
                if (String.IsNullOrEmpty(field.Text))
                {
                    valid = false;
                }
            }
                
            return valid;
        }

        private void dgvStudent_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

    }
}