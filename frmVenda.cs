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
        // String de conecção com o banco de dados
        SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Aluno\\source\\repos\\PedroWatermann\\vendas\\dbVenda.mdf;Integrated Security=True");

        public frmVenda()
        {
            InitializeComponent();
        }

        // Fecha este formulário ao clique do botão
        private void btnVoltar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Carrega as opções do combo box produto
        private void CarregaCbxProduto()
        {
            try
            {
                // Verifica se a conecção está aberta
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }

                // Comando SQL para selecionar o id e o nome dos produtos
                string pro = "SELECT Id,nome FROM Produto ORDER BY nome";
                // Objeto da classe SqlCommand que é configurado para estabelecer uma conecção com o banco de dados
                // Os parâmetros do construtor são o comando sql e a string de conecção
                SqlCommand cmd = new SqlCommand(pro, con);
                // Abre a conecção com o banco
                con.Open();
                // Indica que o comando SQL deve ser tratado como uma string de texto (SELECT, INSERT, UPDATE, DELETE)
                cmd.CommandType = CommandType.Text;
                // Objeto da classe SqlDataAdapter que facilita a leitura dos dados do banco sem a necessidade de abrir e fechar a conecção manualmente para cada operação de leitura
                // Funciona em conjunto com o DataSet
                SqlDataAdapter da = new SqlDataAdapter(pro, con);
                // Objeto da classe DataSet que armazena o conteúdo retornado da consulta
                DataSet ds = new DataSet();
                // Utiliza o SqlDataAdapter para preencher o DataSet com uma tabela com os dados da consulta
                da.Fill(ds, "produto");

                // Define o valor de referência de cada item como o id
                cbxProduto.ValueMember = "Id";
                // Define o texto exibido ao usuário
                cbxProduto.DisplayMember = "nome";
                // Conecta o ComboBox à tabela "produto" do DataSet, permitindo que o ComboBox exiba uma lista de produtos obtida do banco de dados.
                cbxProduto.DataSource = ds.Tables["produto"];

                // Fecha a conecção
                con.Close();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
        }

        // É executado ao carregar o formulário
        private void frmVenda_Load(object sender, EventArgs e)
        {
            // Chama o método para carregar o combo box
            CarregaCbxProduto();
            // Limpa todos os campos
            cbxProduto.Text = string.Empty;
            txtQuantidade.Enabled = false;
            txtQuantidade.Text = string.Empty;
            txtPreco.Enabled = false;
            txtPreco.Text = string.Empty;
            txtTotal.Enabled = false;
            txtTotal.Text = string.Empty;

            // Desabilita os botões
            btnInserir.Enabled = false;
            btnEditar.Enabled = false;
            btnExcluir.Enabled = false;
            btnVenda.Enabled = false;
            btnLimpar.Enabled = false;

            // Adiciona colunas ao data grid
            // O primeiro parâmetro é o nome interno da coluna, usado pelo código para identificá-la
            // O segundo é o que será exibido para o usuário
            dgvVenda.Columns.Add("Id", "Id");
            dgvVenda.Columns.Add("Produto", "Produto");
            dgvVenda.Columns.Add("Quantidade", "Quantidade");
            dgvVenda.Columns.Add("Preco", "Preço");
            dgvVenda.Columns.Add("Total", "Total");
        }

        // É executado quando o índice do combo box é mudado, ou seja, quando o usuário clica em uma opção
        private void cbxProduto_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Verifica se a conecção está aberta
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                // Prepara uma consulta SQL onde o Id é igual a @Id
                SqlCommand cmd = new SqlCommand("SELECT * FROM Produto WHERE Id=@Id", con);
                // Define o valor do parâmetro @Id com base na seleção atual do combo box, que corresponde ao id do produto.
                cmd.Parameters.AddWithValue("@Id", cbxProduto.SelectedValue);
                // Define o tipo do comando como texto
                cmd.CommandType = CommandType.Text;
                con.Open();
                // Executa a consulta e armazena o resultado no SqlDataReader dr.
                SqlDataReader dr = cmd.ExecuteReader();
                // Verifica se a consulta retornou algum dado e avança para a próxima linha, se houver
                if (dr.Read())
                {
                    txtQuantidade.Enabled = true;
                    btnInserir.Enabled = true;
                    btnEditar.Enabled = true;
                    btnExcluir.Enabled = true;
                    btnLimpar.Enabled = true;
                    // Define o campo preco com o valor do campo preço obtido na consulta
                    txtPreco.Text = dr["preco"].ToString();
                    txtQuantidade.Focus();
                }
                // Fecha o SqlDataReader após a leitura
                dr.Close();
                con.Close();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
        }

        // Insere os produtos ao data grid, para contabilizar as vendas
        private void btnInserir_Click(object sender, EventArgs e)
        {
            // Verifica se o campo quantidade está vazio
            if (txtQuantidade.Text == string.Empty)
            {
                MessageBox.Show("Digite a quantidade do produto!", "Quantidade Inválida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtQuantidade.Focus();
                return;
            }

            // Verifica se o campo quantidade está preenchido com 0
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
                // Verifica se a conecção está aberta
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }

                // Prepara o comando sql
                SqlCommand cmd = new SqlCommand("SELECT * FROM Produto WHERE Id = @Id", con);
                // Define o valor de @Id
                cmd.Parameters.AddWithValue("@Id", cbxProduto.SelectedValue);
                // Abre a conecção
                con.Open();
                // Executa a leitura
                SqlDataReader dr = cmd.ExecuteReader();

                // Verifica se a consulta retornou algo
                if (dr.Read())
                {
                    // Verifica se a quantidade digitada pelo usuário é maior que a disponível em estoque
                    if (Convert.ToInt32(txtQuantidade.Text.Trim()) > Convert.ToInt32(dr[2]))
                    {
                        MessageBox.Show("Quantidade indisponível! \nDigite um valor menor.", "Quantidade", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtQuantidade.Focus();
                        txtQuantidade.SelectAll();
                        return;
                    }
                }
                dr.Close();
                con.Close();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }

            // Verifica se o produto já está inserido no data grid, ou seja, já está contabilizado para venda
            foreach (DataGridViewRow dr in dgvVenda.Rows)
            {
                // Verifica se o valor selecionado no combo box é o mesmo valor preenchido na primeira linha do data grid
                if (Convert.ToString(cbxProduto.SelectedValue) == dr.Cells[0].Value.ToString())
                {
                    MessageBox.Show("Produto já cadastrado!", "Produto Repetido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Objeto da classe DataGridViewRow que serve para criar novas células no data grid, formando uma nova linha
            DataGridViewRow item = new DataGridViewRow();
            //  Inicializa as células da linha item com a estrutura do data grid. Isso cria a quantidade correta de células conforme o número de colunas
            item.CreateCells(dgvVenda);
            // Atribui valores específicos a cada célula
            item.Cells[0].Value = cbxProduto.SelectedValue;
            item.Cells[1].Value = cbxProduto.Text;
            item.Cells[2].Value = txtQuantidade.Text;
            item.Cells[3].Value = txtPreco.Text;
            item.Cells[4].Value = Convert.ToDecimal(txtQuantidade.Text) * Convert.ToDecimal(txtPreco.Text);
                
            // Adiciona a linha item preenchida ao data grid
            dgvVenda.Rows.Add(item);

            // Limpa os campos
            cbxProduto.Text = string.Empty;
            txtQuantidade.Text = string.Empty;
            txtPreco.Text = string.Empty;

            btnVenda.Enabled = true;

            // Realiza a soma dos valores de todos os produtos do data grid
            decimal soma = 0;
            foreach (DataGridViewRow dr in dgvVenda.Rows) 
                soma += Convert.ToDecimal(dr.Cells[4].Value);
            txtTotal.Text = soma.ToString();
        }

        // Exibe os valores das células nos campos correspondentes
        private void dgvVenda_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = this.dgvVenda.Rows[e.RowIndex];
            cbxProduto.Text = row.Cells[1].Value.ToString();
            txtQuantidade.Text = row.Cells[2].Value.ToString();
            txtPreco.Text = row.Cells[3].Value.ToString();
            txtQuantidade.Select();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            // Verifica se a conecção está aberta
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }

            // Prepara o comando sql
            SqlCommand cmd = new SqlCommand("SELECT * FROM Produto WHERE Id = @Id", con);
            // Define o valor de @Id
            cmd.Parameters.AddWithValue("@Id", cbxProduto.SelectedValue);
            // Abre a conecção
            con.Open();
            // Executa a leitura
            SqlDataReader dr = cmd.ExecuteReader();

            // Verifica se a consulta retornou algo
            if (dr.Read())
            {
                // Verifica se a quantidade digitada pelo usuário é maior que a disponível em estoque
                if (Convert.ToInt32(txtQuantidade.Text.Trim()) > Convert.ToInt32(dr[2]))
                {
                    MessageBox.Show("Quantidade indisponível! \nDigite um valor menor.", "Quantidade", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQuantidade.Focus();
                    txtQuantidade.SelectAll();
                    return;
                }
            }
            dr.Close();
            con.Close();

            // Verifica se o campo quantidade está preenchido com 0
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

            // Captura o index da linha selecionada
            int linha = dgvVenda.CurrentRow.Index;
            // Adiciona os valores correspondentes
            dgvVenda.Rows[linha].Cells[0].Value = cbxProduto.SelectedValue;
            dgvVenda.Rows[linha].Cells[1].Value = cbxProduto.Text;
            dgvVenda.Rows[linha].Cells[2].Value = txtQuantidade.Text.Trim();
            dgvVenda.Rows[linha].Cells[3].Value = txtPreco.Text.Trim();
            dgvVenda.Rows[linha].Cells[4].Value = Convert.ToDecimal(txtQuantidade.Text.Trim()) * Convert.ToDecimal(txtPreco.Text.Trim());
            
            // Limpa os campos
            cbxProduto.Text = string.Empty;
            txtQuantidade.Text = string.Empty;
            txtPreco.Text = string.Empty;

            // Calcula o valor total
            decimal soma = 0;
            foreach (DataGridViewRow dgvr in dgvVenda.Rows)
                soma += Convert.ToDecimal(dgvr.Cells[4].Value);
            txtTotal.Text = soma.ToString();
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            cbxProduto.Text = string.Empty;
            txtQuantidade.Text = string.Empty;
            txtPreco.Text = string.Empty;
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            int linha = dgvVenda.CurrentRow.Index;
            // dgvVenda.Rows.Remove(dgvVenda.Rows[linha]);
            dgvVenda.Rows.RemoveAt(linha);

            cbxProduto.Text = string.Empty;
            txtQuantidade.Text = string.Empty;
            txtPreco.Text = string.Empty;

            btnEditar.Enabled = false;
            btnExcluir.Enabled = false;
            btnLimpar.Enabled = false;

            decimal soma = 0;
            foreach (DataGridViewRow dr in dgvVenda.Rows)
                soma += Convert.ToDecimal(dr.Cells[4].Value);
            txtTotal.Text = soma.ToString();
        }

        private void btnVenda_Click(object sender, EventArgs e)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCommand cmd = new SqlCommand("InserirVenda", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@total", SqlDbType.Decimal).Value = Convert.ToDecimal(txtTotal.Text.Trim());
            cmd.Parameters.AddWithValue("@data_venda", SqlDbType.Date).Value = DateTime.Now;
            cmd.ExecuteNonQuery();

            string idVenda = "SELECT IDENT_CURRENT('Vendas') AS id_venda";
            SqlCommand cmd2 = new SqlCommand(idVenda, con);
            // Retorna a primeira coluna da ultima linha
            Int32 idVenda2 = Convert.ToInt32(cmd2.ExecuteScalar());

            foreach (DataGridViewRow dr in dgvVenda.Rows)
            {
                SqlCommand cmdItens = new SqlCommand("InserirItensVendidos", con);
                cmdItens.CommandType = CommandType.StoredProcedure;
                cmdItens.Parameters.AddWithValue("@id_venda", SqlDbType.Int).Value = idVenda2;
                cmdItens.Parameters.AddWithValue("@id_produto", SqlDbType.Int).Value = Convert.ToInt32(dr.Cells[0].Value);
                cmdItens.Parameters.AddWithValue("@quantidade", SqlDbType.Int).Value = Convert.ToInt32(dr.Cells[2].Value);
                cmdItens.Parameters.AddWithValue("@valor_unitario", SqlDbType.Decimal).Value = Convert.ToDecimal(dr.Cells[3].Value);
                cmdItens.Parameters.AddWithValue("@total", SqlDbType.Decimal).Value = Convert.ToDecimal(dr.Cells[4].Value);
                cmdItens.ExecuteNonQuery();
            }

            dgvVenda.Rows.Clear();
            dgvVenda.Refresh();
            
            cbxProduto.Text = string.Empty;
            txtQuantidade.Text = string.Empty;
            txtPreco.Text = string.Empty;
            txtTotal.Text = string.Empty;
            
            btnVenda.Enabled = false;
            btnEditar.Enabled = false;
            btnExcluir.Enabled = false;
            btnLimpar.Enabled = false;
            
            MessageBox.Show("Venda realizada com sucesso!", "Venda", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
