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

        private void btnLocalizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtId.Text == string.Empty)
                {
                    MessageBox.Show("Por favor, digite um Id válido!", "Localizar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.ActiveControl = txtId;
                    return;
                }
                else
                {
                    ConProduto conProduto = new ConProduto();
                    int Id = Convert.ToInt32(txtId.Text.Trim());
                    conProduto.Localizar(Id);
                    txtNome.Text = conProduto.nome;
                    txtPreco.Text = Convert.ToString(conProduto.preco);
                    txtQuantidade.Text = Convert.ToString(conProduto.quantidade);
                    btnEditar.Enabled = true;
                    btnExcluir.Enabled = true;
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtId.Text == string.Empty || txtNome.Text == string.Empty || txtQuantidade.Text == string.Empty || txtPreco.Text == string.Empty)
                {
                    MessageBox.Show($"Por favor, preencha todos os campos!", "Campo Obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    int Id = Convert.ToInt32(txtId.Text.Trim());
                    int quntidade = Convert.ToInt32(txtQuantidade.Text.Trim());
                    ConProduto conProduto = new ConProduto();
                    conProduto.Atualizar(Id, txtNome.Text, quntidade, txtPreco.Text);
                    MessageBox.Show("Produto atualizado com sucesso!", "Atualização", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    List<Produto> produtos = conProduto.ListaProduto();
                    dgvProduto.DataSource = produtos;

                    txtId.Text = string.Empty;
                    txtNome.Text = string.Empty;
                    txtPreco.Text = string.Empty;
                    txtQuantidade.Text = string.Empty;
                    btnEditar.Enabled = false;
                    btnExcluir.Enabled = false;
                    this.ActiveControl = txtNome;
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtId.Text == string.Empty)
                {
                    MessageBox.Show($"Por favor, preencha um Id válido!", "Id", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    string produto = txtNome.Text;
                    DialogResult resultado = MessageBox.Show($"Deseja mesmo excluir o produto {produto}?", "Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (resultado == DialogResult.Yes)
                    {
                        int Id = Convert.ToInt32(txtId.Text.Trim());

                        ConProduto conProduto = new ConProduto();
                        conProduto.Excluir(Id);

                        MessageBox.Show("Produto excluído com sucesso!", "Exclusão", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        List<Produto> produtos = conProduto.ListaProduto();
                        dgvProduto.DataSource = produtos;

                        txtId.Text = string.Empty;
                        txtNome.Text = string.Empty;
                        txtPreco.Text = string.Empty;
                        txtQuantidade.Text = string.Empty;
                        btnEditar.Enabled = false;
                        btnExcluir.Enabled = false;
                        this.ActiveControl = txtNome;
                    }
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtPreco_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '.')
            {
                MessageBox.Show("Apenas vírgula ( , ) é permitido!", "Tecla Pressionada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.KeyChar = ',';
            }
        }

        private void dgvProduto_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = this.dgvProduto.Rows[e.RowIndex];
                this.dgvProduto.Rows[e.RowIndex].Selected = true;
                txtId.Text = row.Cells[0].Value.ToString();
                txtNome.Text = row.Cells[1].Value.ToString();
                txtPreco.Text = row.Cells[2].Value.ToString();
                txtQuantidade.Text = row.Cells[3].Value.ToString();
            }
            btnEditar.Enabled = true;
            btnExcluir.Enabled = true;
        }

        private void btnVoltar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
