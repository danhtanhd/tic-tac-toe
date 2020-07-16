using Caro.Business.Entities;
using Caro.Business.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Caro
{
    public partial class frmMain : Form
    {
        #region Properties
        public static bool language =true; // kiểm tra ngôn ngữ
        ChessBoardManager chessboardmanager; // lớp quản lí bàn cờ
        public static int chessboard_Width; // chiều rộng bàn cờ(Panel)
        public static int chessboard_Hight;// Chiều Dài bàn cờ (panel)
        public static int size; // Kích thước bàn cờ
        public static TextBox mainTextBox; // Nơi nhập kích thước
        public static Button btn_Start; //nút bắt đầu
        CultureInfo culture; // biến để truyền giá trị ngôn ngữ

        #endregion

        public frmMain()
        {
            InitializeComponent();

            chessboardmanager = new ChessBoardManager(pnlChessBoard);//khởi tạo đối tượng điều khiển trò chơi

            //lấy chiều rộng và chiều cao pnBanCo để vẽ bàn cờ
            chessboard_Width = pnlChessBoard.Width;
            chessboard_Hight = pnlChessBoard.Height;
        }

        private void txtSize_KeyPress(object sender, KeyPressEventArgs e)// hàm kiểm tra chỉ cho nhập số
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        public static void NewGame() //hàm thiết lập cho phép nhập kích thước và cho nhất nút bắt đầu
        {
            btn_Start.Enabled=true;
            mainTextBox.ReadOnly = false;
        }

        private void btnStart_Click(object sender, EventArgs e)// game mới
        {
            ResourceManager rm = new ResourceManager("Caro.Lang.resource", typeof(frmMain).Assembly);
            txtSize.ReadOnly = false; // khóa ô nhập kích thước
            if(Convert.ToInt32(txtSize.Text)<6)
            {
                //gán string cho biến để hiển thị lên messageBox
                string mess = rm.GetString("Min", culture);
                MessageBox.Show(mess);
                return;
            }
            // tính toán chiều rộng và cao của một ô cờ
            ChessBoard_Box.box_height = pnlChessBoard.Height / Convert.ToInt32(txtSize.Text);
            ChessBoard_Box.box_width = pnlChessBoard.Width / Convert.ToInt32(txtSize.Text);
            size = Convert.ToInt32(txtSize.Text);// truyền kích thước ô cơ vào biến
            pnlChessBoard.Controls.Clear();// xóa các ô cờ đã in để in ô mới
            chessboardmanager.DrawChessBoard(Convert.ToInt32(txtSize.Text));// vẽ bàn cờ theo kích thước đã cho

            //chọn chế độ chơi
            if (rdoPVE.Checked)// chế độ người chơi với máy game_mode =2
            {
                chessboardmanager.Game_mode = 2;
                chessboardmanager.PVE();
                btnStart.Enabled = false;
                chessboardmanager.IsGameOver = false;
            }
            if (rdoPVP.Checked) //chế độ người chơi với người game_mod=1
            {
                chessboardmanager.Game_mode = 1;
                chessboardmanager.PVP();
                btnStart.Enabled = false;
                chessboardmanager.IsGameOver = false;
            }

}

        private void btnSave_Click(object sender, EventArgs e)// lưu game
        {
            chessboardmanager.SaveGame();
        }

        private void btnload_Click(object sender, EventArgs e)// load game đã lưu
        {
            pnlChessBoard.Controls.Clear();
            chessboardmanager.LoadGame(pnlChessBoard.Width, pnlChessBoard.Height);
        }

        private void btnExit_Click(object sender, EventArgs e) //thoát khỏi phần chơi game
        {
            //set ngôn ngữ
            if (language)
            {
                culture = CultureInfo.CreateSpecificCulture("en-US");
            }
            else
            {
                culture = CultureInfo.CreateSpecificCulture("ja-JP");
            }

            // khởi tạo biến lấy dữ liệu ngôn ngữ từ resource
            ResourceManager rm = new ResourceManager("Caro.Lang.resource", typeof(frmMain).Assembly);
            //gán string cho biến để hiển thị lên messageBox
            string mess = rm.GetString("Askexit",culture);
            string messwar = rm.GetString("Warning", culture);
            if (!chessboardmanager.IsGameOver)// nếu game chưa kết thúc hỏi người chơi có muốn kế thúc không nếu không chơi tiếp
            {
                DialogResult dialogResult = MessageBox.Show(mess, messwar, MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    pnlChessBoard.Controls.Clear();
                    txtSize.ReadOnly = false;
                    btnStart.Enabled = true;
                    chessboardmanager.IsGameOver = true;
                }
            }
            else
            {
                pnlChessBoard.Controls.Clear();
                txtSize.ReadOnly = false;
                btnStart.Enabled = true;
            }
        }

        private void SetLanguage(string cultureName)
        {
            // khởi tạo biến lấy dữ liệu ngôn ngữ từ resource
            culture = CultureInfo.CreateSpecificCulture(cultureName);
            ResourceManager rm = new
                ResourceManager("Caro.Lang.resource", typeof(frmMain).Assembly);
            // gán string cho lable và button
            mnLanguage.Text = rm.GetString("Language", culture);
            lblGamemode.Text= rm.GetString("Gamemode", culture);
            lblSize.Text = rm.GetString("Size", culture);
            btnStart.Text = rm.GetString("Start", culture);
            btnExit.Text = rm.GetString("Endgame", culture);
            btnSave.Text= rm.GetString("Save", culture);
            btnload.Text = rm.GetString("Load", culture);
            btnSave.Text = rm.GetString("Save", culture);
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            //gán biến tĩnh để sử dụng ở lớp khác
            mainTextBox = new TextBox();
            btn_Start = new Button();
            mainTextBox = this.txtSize;
            btn_Start = this.btnStart;
        }

        private void englishToolStripMenuItem1_Click(object sender, EventArgs e)// nút đổi ngôn ngữ
        {
            // trạng thái ngôn ngữ false là tiếng nhật, true là tiếng anh
            if (language)
            {
                SetLanguage("ja-JP");
                ChessBoardManager.culture = CultureInfo.CreateSpecificCulture("ja-JP");
                language = false;
            }
            else
            {
                SetLanguage("en-US");
                ChessBoardManager.culture = CultureInfo.CreateSpecificCulture("en-US");
                language = true;
            }
        }
    }
}
