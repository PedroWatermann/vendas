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

        // Método para mudar o estado dos botões editar e excluir
        private void ButtonsState(bool value)
        {
            btnEditar.Enabled = value;
            btnExcluir.Enabled = value;
            btnLimpar.Enabled = value;
        }

        // Método para limpar os campos nome, quantidade e preço
        private void ClearFields()
        {
            txtNome.Text = string.Empty;
            txtQuantidade.Text = string.Empty;
            txtPreco.Text = string.Empty;
        }

        // Método que executa no carregamento do form
        private void frmProduto_Load(object sender, EventArgs e)
        {
            // Controller instanciado
            ConProduto conProduto = new ConProduto();
            // Cria uma lista a partir dos valores existentes no método ListaProduto
            List<Produto> produtos = conProduto.ListaProduto();
            // Exibe a lista criado no data grid view
            dgvProduto.DataSource = produtos;
            // Chama o método para desabilitar os botões
            ButtonsState(false);
        }

        // Insere os dados ao ser clicado
        private void btnInserir_Click(object sender, EventArgs e)
        {
            // Bloco de tratamento de erro
            try
            {
                // Verifica se há algum campo vazio
                if (txtNome.Text == "" || txtQuantidade.Text == "" || txtPreco.Text == "")
                {
                    MessageBox.Show($"Por favor, preencha todos os campos!", "Campo Obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    // Instancia o controller
                    ConProduto conProduto = new ConProduto();
                    // Verifica se o nome do produto é repetido
                    if (conProduto.RegistroRepetido(txtNome.Text) == true)
                    {
                        MessageBox.Show($"\"{txtNome.Text}\" já existe em nossa base de dados!", "Produto Repetido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearFields();
                        return;
                    } 
                    else
                    {
                        int quantidade = Convert.ToInt32(txtQuantidade.Text.Trim());
                        // Verifica se a quantidade é menor ou igual a zero
                        if (quantidade <= 0)
                        {
                            MessageBox.Show("A quantidade deve ser maior que zero (0)!", "Quantidade", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.ActiveControl = txtQuantidade;
                            return;
                        }
                        else // Caso tudo passe pelas verificações o produto é inserido
                        {
                            conProduto.Inserir(txtNome.Text, quantidade, txtPreco.Text);
                            MessageBox.Show("Produto inserido com sucesso!", "Inserção", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // Limpa os campos
                            ClearFields();
                            // Cria uma lista a partir do método ListaProduto do controller
                            List<Produto> produtos = conProduto.ListaProduto();
                            // Atualiza o data grid com o novo registro dos produtos
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

        // Realiza a pesquisa dos registros
        private void btnLocalizar_Click(object sender, EventArgs e)
        {
            // Tratamento de erro
            try
            {
                // Verifica se o campo id está vazio
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
                    // Utiliza o método Localizar como o parâmetro id
                    conProduto.Localizar(Id);
                    // Preenche os campos com as informações correspondentes
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

        // Edita os registros
        private void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                // Verifica se todos os campos estao preenchidos
                if (txtId.Text == string.Empty || txtNome.Text == string.Empty || txtQuantidade.Text == string.Empty || txtPreco.Text == string.Empty)
                {
                    MessageBox.Show($"Por favor, preencha todos os campos!", "Campo Obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    int Id = Convert.ToInt32(txtId.Text.Trim());
                    int quntidade = Convert.ToInt32(txtQuantidade.Text.Trim());

                    // Utiliza o método Atualizar do controller
                    ConProduto conProduto = new ConProduto();
                    conProduto.Atualizar(Id, txtNome.Text, quntidade, txtPreco.Text);

                    MessageBox.Show("Produto atualizado com sucesso!", "Atualização", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Atualiza o data grid
                    List<Produto> produtos = conProduto.ListaProduto();
                    dgvProduto.DataSource = produtos;

                    // Limpa os campos e desabilita os botões
                    txtId.Text = string.Empty;
                    ClearFields();
                    ButtonsState(false);
                    this.ActiveControl = txtNome;
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Exclui os registros
        private void btnExcluir_Click(object sender, EventArgs e)
        {
            try
            {
                // Verifica se o campo id está vazio
                if (txtId.Text == string.Empty)
                {
                    MessageBox.Show($"Por favor, preencha um Id válido!", "Id", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    string produto = txtNome.Text;
                    // Exibe uma caixa de confiração para a exclusão do produto
                    DialogResult resultado = MessageBox.Show($"Deseja mesmo excluir o produto {produto}?", "Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    // Caso a resposta seja sim
                    if (resultado == DialogResult.Yes)
                    {
                        // Obtém o id do produto
                        int Id = Convert.ToInt32(txtId.Text.Trim());

                        // Executa o método Excluir com o parâmetro id
                        ConProduto conProduto = new ConProduto();
                        conProduto.Excluir(Id);

                        MessageBox.Show("Produto excluído com sucesso!", "Exclusão", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Atualiza o data grid com as novas informações
                        List<Produto> produtos = conProduto.ListaProduto();
                        dgvProduto.DataSource = produtos;

                        txtId.Text = string.Empty;
                        ClearFields();
                        ButtonsState(false);
                        this.ActiveControl = txtNome;
                    }
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Verifica o separador de decimal
        private void txtPreco_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '.')
            {
                MessageBox.Show("Apenas vírgula ( , ) é permitido!", "Tecla Pressionada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.KeyChar = ',';
            }
        }

        // Preenche os valores correspondesntes nos campos ao clicar na célula do data grid
        // Parâmetros: sender => quem disparou o evento (dgvProduto) / e => argumentos enviados, como índice da linha
        private void dgvProduto_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verifica se o index da linha é maior ou igual a 0, ou seja, se é a primeira linha em diante
            if (e.RowIndex >= 0)
            {
                // Cria uma variável para armazenar o índice da linha, acessado por e.RowIndex
                DataGridViewRow row = this.dgvProduto.Rows[e.RowIndex];

                // Define a linha clicada como selecionada (ajuda visual ao usuário)
                this.dgvProduto.Rows[e.RowIndex].Selected = true;

                // Atribui o valor de cada célula ao seu respectivo campo
                txtId.Text = row.Cells[0].Value.ToString();
                txtNome.Text = row.Cells[1].Value.ToString();
                txtQuantidade.Text = row.Cells[2].Value.ToString();
                txtPreco.Text = row.Cells[3].Value.ToString();
            }
            // Habilita os botões
            ButtonsState(true);
        }

        // Fecha este form
        private void btnVoltar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Botão para limpar os campos
        private void btnLimpar_Click(object sender, EventArgs e)
        {
            txtId.Text = string.Empty;
            ClearFields();
        }
    }
}