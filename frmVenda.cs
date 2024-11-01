using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vendas
{
    public partial class frmVenda : Form
    {
        public frmVenda()
        {
            InitializeComponent();
        }

        SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Aluno\\source\\repos\\PedroWatermann\\vendas\\dbVenda.mdf;Integrated Security=True");

        private void btnVoltar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CarregaCbxProduto()
        {
            try
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                string pro = "SELECT Id,nome FROM Produto ORDER BY nome";
                SqlCommand cmd = new SqlCommand(pro, con);
                con.Open();
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(pro, con);
                DataSet ds = new DataSet();
                da.Fill(ds, "produto");
                cbxProduto.ValueMember = "Id";
                cbxProduto.DisplayMember = "nome";
                cbxProduto.DataSource = ds.Tables["produto"];
                con.Close();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
        }

        private void frmVenda_Load(object sender, EventArgs e)
        {
            CarregaCbxProduto();
            cbxProduto.Text = string.Empty;
            txtQuantidade.Enabled = false;
            txtQuantidade.Text = string.Empty;
            txtPreco.Enabled = false;
            txtPreco.Text = string.Empty;
            txtTotal.Enabled = false;
            txtTotal.Text = string.Empty;
            btnInserir.Enabled = false;
            btnEditar.Enabled = false;
            btnExcluir.Enabled = false;
            btnVenda.Enabled = false;

            dgvVenda.Columns.Add("Id", "Id");
            dgvVenda.Columns.Add("Produto", "Produto");
            dgvVenda.Columns.Add("Quantidade", "Quantidade");
            dgvVenda.Columns.Add("Preco", "Preço");
            dgvVenda.Columns.Add("Total", "Total");
        }

        private void cbxProduto_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                SqlCommand cmd = new SqlCommand("SELECT * FROM Produto WHERE Id=@Id", con);
                cmd.Parameters.AddWithValue("@Id", cbxProduto.SelectedValue);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    txtQuantidade.Enabled = true;
                    btnInserir.Enabled = true;
                    btnEditar.Enabled = true;
                    btnExcluir.Enabled = true;
                    txtPreco.Text = dr["preco"].ToString();
                    txtQuantidade.Focus();
                }
                dr.Close();
                con.Close();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
        }

        private void btnInserir_Click(object sender, EventArgs e)
        {
            if (txtQuantidade.Text == string.Empty)
            {
                MessageBox.Show("Digite a quantidade do produto!", "Quantidade Inválida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtQuantidade.Focus();
                return;
            }

            if (txtQuantidade.Text == "0")
            {
                DialogResult msg = MessageBox.Show("A quantidade foi definida como 0. Deseja continuar?", "Quantidade Inválida", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (msg != DialogResult.Yes)
                {
                    txtQuantidade.Focus();
                    txtQuantidade.SelectAll();
                    return;
                }
            }

            try
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }

                SqlCommand cmd = new SqlCommand("SELECT * FROM Produto WHERE Id = @Id", con);
                cmd.Parameters.AddWithValue("@Id", cbxProduto.SelectedValue);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    if (Convert.ToInt32(txtQuantidade.Text) > Convert.ToInt32(dr[2]))
                    {
                        MessageBox.Show("Quantidade indisponível! \nDigite um valor menor", "Quantidade", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtQuantidade.Focus();
                        txtQuantidade.SelectAll();
                        return;
                    }
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }

            foreach (DataGridViewRow dr in dgvVenda.Rows)
            {
                if (Convert.ToString(cbxProduto.SelectedValue) == dr.Cells[0].Value.ToString())
                {
                    MessageBox.Show("Produto já cadastrado!", "Produto Repetido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            DataGridViewRow item = new DataGridViewRow();
            item.CreateCells(dgvVenda);
            item.Cells[0].Value = cbxProduto.SelectedValue;
            item.Cells[1].Value = cbxProduto.Text;
            item.Cells[2].Value = txtQuantidade.Text;
            item.Cells[3].Value = txtPreco.Text;
            item.Cells[4].Value = Convert.ToDecimal(txtQuantidade.Text) * Convert.ToDecimal(txtPreco.Text);
                
            dgvVenda.Rows.Add(item);
            cbxProduto.Text = string.Empty;
            txtQuantidade.Text = string.Empty;
            txtPreco.Text = string.Empty;

            decimal soma = 0;
            foreach (DataGridViewRow dr in dgvVenda.Rows) 
                soma += Convert.ToDecimal(dr.Cells[4].Value);
            txtTotal.Text = soma.ToString();
        }
    }
}
