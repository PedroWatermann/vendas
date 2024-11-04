using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vendas
{
    public partial class frmMenu : Form
    {
        public frmMenu()
        {
            InitializeComponent();
        }

        // Abre uma caixa de diálogo para confirmação e, caso sim, fecha a aplicação
        private void btnFechar_Click(object sender, EventArgs e)
        {
            var fechar = MessageBox.Show("Deseja realmente sair?", "Sair", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (fechar == DialogResult.Yes)
            {
                Environment.Exit(0);
            }
        }

        // Abre o form produdo quando o item é clicado
        private void pbxProduto_Click(object sender, EventArgs e)
        {
            frmProduto produto = new frmProduto();
            // Esconde este formulário
            this.Hide();
            // Abre o form de forma modal
            produto.ShowDialog();
            // Mostra este form após fechar o produto
            this.Show();
        }

        // Abre o form vendas quando o item é clicado
        private void pbxVendas_Click(object sender, EventArgs e)
        {
            frmVenda venda = new frmVenda();
            // Esconde este formulário
            this.Hide();
            // Abre o form de forma modal
            venda.ShowDialog();
            // Mostra este form após fechar o vendas
            this.Show();
        }
    }
}
