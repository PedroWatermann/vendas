using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vendas.Models;
using vendas.Controller;

namespace vendas
{
    public partial class frmProduto : Form
    {
        public frmProduto()
        {
            InitializeComponent();
        }

        private void ButtonsState(bool value)
        {
            btnEditar.Enabled = value;
            btnExcluir.Enabled = value;
            btnLocalizar.Enabled = value;
            txtId.Enabled = value;
        }

        private void ClearFields()
        {
            txtNome.Text = string.Empty;
            txtQuantidade.Text = string.Empty;
            txtPreco.Text = string.Empty;
        }

        private void frmProduto_Load(object sender, EventArgs e)
        {
            /*Produto produto = new Produto(); // Model*/
            ConProduto conProduto = new ConProduto(); // Controller
            List<Produto> produtos = conProduto.ListaProduto();
            dgvProduto.DataSource = produtos;
            ButtonsState(false);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnInserir_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtNome.Text == "" || txtQuantidade.Text == "" || txtPreco.Text == "")
                {
                    MessageBox.Show($"Por favor, preencha todos os campos!", "Campo Obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    ConProduto conProduto = new ConProduto();
                    if (conProduto.RegistroRepetido(txtNome.Text) == true)
                    {
                        MessageBox.Show($"\"{txtNome.Text}\" já existe em nossa base de dados!", "Produto Repetido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearFields();
                        return;
                    } 
                    else
                    {
                        int quantidade = Convert.ToInt32(txtQuantidade.Text.Trim());
                        if (quantidade <= 0)
                        {
                            MessageBox.Show("A quantidade deve ser maior que zero (0)!", "Quantidade", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.ActiveControl = txtQuantidade;
                            return;
                        }
                        else
                        {
                            conProduto.Inserir(txtNome.Text, quantidade, txtPreco.Text);
                            MessageBox.Show("Produto inserido com sucesso!", "Inserção", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearFields();
                            List<Produto> produtos = conProduto.ListaProduto();
                            dgvProduto.DataSource = produtos;
                        }
                    }
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
