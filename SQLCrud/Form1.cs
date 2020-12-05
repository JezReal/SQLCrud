using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace SQLCrud
{
    public partial class Form1 : Form
    {
        //DATABASE STRING SYNTAX, PASSWORD MAY BE LEFT EMPTY OR NOT CODED ENTIRELY
        string connectionString = @"Server=localhost;Database=studentdb;Uid=root;";
        int StudentID = 0;
        
        private bool isOperationUpdate;

        public Form1()
        {
            InitializeComponent();
        }

        //09-Nov-2019 BSIT 2A/2B
        //ADDING OF RECORDS
        private void BtnSave_Click(object sender, EventArgs e)
        {
            using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
            {
                mySqlConnection.Open();
                MySqlCommand mySqlCmd = new MySqlCommand("StudentAddOrEdit", mySqlConnection);
                mySqlCmd.CommandType = CommandType.StoredProcedure;
                mySqlCmd.Parameters.AddWithValue("_StudentID", StudentID);
                mySqlCmd.Parameters.AddWithValue("_StudentName", txtStudentName.Text.Trim());
                mySqlCmd.Parameters.AddWithValue("_StudentAddress", txtAddress.Text.Trim());
                mySqlCmd.Parameters.AddWithValue("_Description", txtDescription.Text.Trim());
                mySqlCmd.ExecuteNonQuery();
                if (isOperationUpdate)
                {
                    MessageBox.Show("Student has been updated");
                }
                else
                {
                    MessageBox.Show("Student has been added");
                }

                isOperationUpdate = false;

                Clear();
                GridFill();
            }
        }


        private void Form1_Load_1(object sender, EventArgs e)
        {
            
            Clear(); //CLEARS RECORD TO GIVE WAY FOR THE POPULATION OF FRESH RECORDS (GRIDFILL)
            GridFill(); //POPULATES THE DATA GRID VIEW OF RECORDS FROM OUR DATABASE
        }


        //POPULATING OF DATA GRID VIEW WITH DATABASE RECORDS
        void GridFill()
        {
            using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
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
        void Clear()
        {
            txtStudentName.Text = txtAddress.Text = txtDescription.Text = txtSearch.Text = "";
            StudentID = 0;
            btnSave.Text = "Save";
            btnDelete.Enabled = false;
        }

        //ALLOWS SELECTION OF RECORDS INSIDE THE DATA GRID VIEW
        private void dgvStudent_DoubleClick(object sender, EventArgs e)
        {
            if (dgvStudent.CurrentRow.Index != -1)
            {
                txtStudentName.Text = dgvStudent.CurrentRow.Cells[1].Value.ToString();
                txtAddress.Text = dgvStudent.CurrentRow.Cells[2].Value.ToString();
                txtDescription.Text = dgvStudent.CurrentRow.Cells[3].Value.ToString();
                StudentID = Convert.ToInt32(dgvStudent.CurrentRow.Cells[0].Value.ToString());
                btnSave.Text = "Update";
                btnDelete.Enabled = Enabled;
                isOperationUpdate = true;
            }
        }

        //ADDING OF SEARCH FILTER TO OUR DATA GRID VIEW FOR FASTER ACCESS TO A SPECIFIC RECORD 
        // AND TO SIMULATE ANOTHER READING OPERATION ON SQL (PLEASE REFER TO THE ROUTINE)
        private void btnSearch_Click(object sender, EventArgs e)
        {
            using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
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

        //CLICKING THE CANCEL BUTTON TRIGGERS OUR CLEAR METHOD
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        //PLEASE REFER TO OUR ROUTINE, THIS DELETES THE RECORD CURRENTLY SELECTED
        private void btnDelete_Click(object sender, EventArgs e)
        {
            using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
            {
                mySqlConnection.Open();
                MySqlCommand mySqlCmd = new MySqlCommand("StudentDeleteByID", mySqlConnection);
                mySqlCmd.CommandType = CommandType.StoredProcedure;
                mySqlCmd.Parameters.AddWithValue("_StudentID", StudentID);
                mySqlCmd.ExecuteNonQuery();
                MessageBox.Show("Deleted Successfully");
                Clear();
                GridFill();
            }
        }

        
    }
}