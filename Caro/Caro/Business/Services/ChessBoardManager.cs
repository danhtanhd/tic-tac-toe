using Caro.Business.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Caro.Business.Services
{
    class ChessBoardManager
    {
        #region Properties
        // đối tượng tĩnh có thể thiết lập trực tiếp từ lớp khác mà ko phải khởi tạo đối tượng
        public static CultureInfo culture;
        // biến dùng để đọc ghi file
        private StreamWriter writeFile;
        private StreamReader readFile;

        //biến để lấy resource
        private ResourceManager rm;

        //panel để vẽ bàn cờ
        private Panel chessBoard;

        //hình X-O
        private Image ImageO;
        private Image ImageX;

        // biến check game đã kết thúc chưa 
        private bool isGameOver = false;

        // các hàm truy cập thiết lập giá trị thuộc tính bán đầu
        public int Game_mode { get => game_mode; set => game_mode = value; }
        public int Turn { get => turn; set => turn = value; }
        public bool Ready { get => ready; set => ready = value; }
        public bool IsGameOver { get => isGameOver; set => isGameOver = value; }

        // mảng ô cờ
        private ChessBoard_Box[,] chess_board_box_arr;
        private Random rd = new Random();//random ô cờ máy sẽ đánh đầu tiên, và random lượt đi đầu tiên khi chơi PVP
        private int game_mode;// chế độ chơi
        private int turn;// lượt chơi
        private bool ready;// trạng thai kiểm tra xem đã có thể choi hay chưa
        private Stack<ChessBoard_Box> sta_player_turn;// stack để chứa ô đánh để cài đặt màu sắc
        private Stack<ChessBoard_Box> sta_buttonClicked;// lưu lại lượt chơi đến hiện tại
        #endregion

        #region Initialize
        public ChessBoardManager(Panel chessBoard) //khởi tạo
        {
            // thiết lập biến resource
            rm = new
                ResourceManager("Caro.Lang.resource", typeof(frmMain).Assembly);
            this.chessBoard = chessBoard;
            var assembly = Assembly.GetExecutingAssembly();     //lấy nguồn
            // biến để đọc hình ảnh X và O
            System.IO.Stream stream_o = null;
            System.IO.Stream stream_x = null;
            //dọc ảnh từ resource và gán vào biến
            stream_o = assembly.GetManifestResourceStream(@"Caro.GUI.Image.o.png");
            stream_x = assembly.GetManifestResourceStream(@"Caro.GUI.Image.x.png");
            ImageO = new Bitmap(stream_o);
            ImageX = new Bitmap(stream_x);

            // thiết lâp các stack để chứa dữ liệu
            sta_buttonClicked = new Stack<ChessBoard_Box>();
            sta_player_turn = new Stack<ChessBoard_Box>();
            isGameOver = false;
        }

        #endregion

        #region Method

        public void DrawChessBoard(int sizeChessBoard)// vẽ ô cờ
        {
            ChessArrayBox(sizeChessBoard);//tạo mảng ô cơ dựa vào kích thước cho trước
            ChessBoard_Box oldButton = new ChessBoard_Box()
            {
                Width = 0,
                Location = new Point(0, 0)
            };//tạo ô cờ ảo ban đầu để bắt đầu vẽ
            
            for (int i = 0; i < sizeChessBoard; i++)//duyêt từng hàng từng cột để vẽ ô cờ
            {
                for (int j = 0; j <= sizeChessBoard; j++)
                {
                    ChessBoard_Box btn = new ChessBoard_Box(i, j, 0)//tao ô cờ với giá trị vị trí và chưa người sở hữu là 
                    {
                        Width = ChessBoard_Box.box_width,//set kick thước đã được tính toán
                        Height = ChessBoard_Box.box_height,//set kick thước đã được tính toán
                        Location = new Point(oldButton.Location.X + oldButton.Width, oldButton.Location.Y),//set vị trí liền kề với ô phía trước
                        BackgroundImageLayout = ImageLayout.Stretch// căn lại hình ảnh khi hiển thị    
                    };
                    btn.Click += Btn_Click;// thiết lập sự kiện click cho ô cờ
                    chessBoard.Controls.Add(btn);//thêm ô cờ vào panel
    

                    oldButton = btn;// đổi vị trí ô cuối thành ô vừa tạo
                    if (j<sizeChessBoard)//chỉ lưu giá trị đúng với kích thước vào mảng ô cờ
                    {
                        chess_board_box_arr[i, j] = btn;
                    }
                }
                // thiết lập ô cuối của dòng để xuống dòng
                oldButton.Location = new Point(0, oldButton.Location.Y + ChessBoard_Box.box_height);
                oldButton.Width = 0;
                oldButton.Height = 0;
            }

        }

        public void SaveGame()// lưu game
        {
            if (ready)// chỉ lưu khi đã đánh cờ
            {
                try
                {
                    File.WriteAllText("SaveGame", String.Empty);// xóa dữ liệu cũ
                    writeFile = new StreamWriter("SaveGame", true);// chọn tệp SaveGame để ghi dữ liệu
                    writeFile.WriteLine(frmMain.size.ToString());// lấy kích thước bàn cờ
                    Stack<ChessBoard_Box> sta_player_turn_tmp = sta_buttonClicked;// lấy một stack lượt chơi để lưu

                    ChessBoard_Box turnTmp = new ChessBoard_Box();// tạo ô cờ mời để sử lí duwxx liệu lấy từ stack
                    string tmpString;// chuỗi ghi xuống tệp
                    int index = sta_player_turn_tmp.Count;// lấy ra số nước đi
                    for (int i = 0; i < index; i++)// với mỗi nước đi tiến hành sử lí cho đến khi không còn nước đi
                    {
                        turnTmp = sta_player_turn_tmp.Pop();// lấy ra nước đi cuối cùng
                        tmpString = turnTmp.Row.ToString() + "," + turnTmp.Cloumn.ToString() + "," + turnTmp.Owner.ToString();// tạo chuỗi ghi dữ liệu
                        if (i == (index - 1))// phần tử ghi cuối cùng không cần kí tự thêm
                        {
                            writeFile.Write(tmpString);
                        }
                        else// các phần tử khác cần dấu hiệu để cắt
                        {
                            writeFile.Write(tmpString + "|");
                        }
                    }
                    writeFile.Dispose();// hủy phương thức ghi
                    string messdone = rm.GetString("Savedone", culture);// lấy dữ liệu chuỗi theo ngôn ngữ đã chọn và mã chuỗi
                    MessageBox.Show(messdone);// thông báo ghi thành công
                }
                catch (Exception)
                {
                    // thông báo ghi thất bại
                    writeFile.Dispose();
                    string messerror = rm.GetString("Cantsave", culture); // lấy dữ liệu chuỗi theo ngôn ngữ đã chọn và mã chuỗi
                    MessageBox.Show(messerror); // thông báo ghi thất bại
                }

            }

            else// nếu chưa đanh cờ yêu cầu người chơi start
            {
                string messNotYet = rm.GetString("Plesestart", culture);
                MessageBox.Show(messNotYet);
            }
        }
        public void LoadGame(int width, int height)// load dữ liệu đã lưu
        {
            readFile = new StreamReader("SaveGame");// đọc dữ liệu từ file SaveGame
            string tmp = readFile.ReadToEnd(); // lưu chuỗi cắt được
            string[] lines = tmp.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
            );// cắt dữ liệu theo dòng
            try// sử lí dữ liệu
            {
                // gán lại kích thước cũ
                frmMain.size = Convert.ToInt32(lines[0]); // kích thước bàn cơ theo phần tử  đầu
                ChessBoard_Box.box_height = height / Convert.ToInt32(frmMain.size);
                ChessBoard_Box.box_width = width / Convert.ToInt32(frmMain.size);
                DrawChessBoard(frmMain.size); // vẽ lại bàn cờ kích thước cũ nhưng trắng
                
                // cắt phần tử thứ 2 theo kí tự '|'
                string[] chessrow = lines[1].Split('|');
                for (int j = chessrow.Length-1; j >0 ; j--) // duyệt chuỗi cắt được để tính toán nước đi
                {
                    string[] tmpIndex = chessrow[j].Split(',');//cắt tiếp chuỗi sau khi cắt theo kí tự '|'
                    turn = Convert.ToInt32(tmpIndex[2]);// ghi nhận quyền sở hữu ô đó
                    ChessBoard_Box tmpChessBox = new ChessBoard_Box(Convert.ToInt32(tmpIndex[0]), Convert.ToInt32(tmpIndex[1]), Convert.ToInt32(tmpIndex[2]));// tạo ô cờ mới theo thông tin cắt được
                    sta_player_turn.Push(tmpChessBox);// đẩy vào stack
                    sta_buttonClicked.Push(tmpChessBox);
                    if (turn == 1)// nếu ô cờ thuộc máy thì gán O và thiết lập hiện thị
                    {
                        chess_board_box_arr[Convert.ToInt32(tmpIndex[0]), Convert.ToInt32(tmpIndex[1])].Owner = turn;// gán người sở hữu
                        chess_board_box_arr[Convert.ToInt32(tmpIndex[0]), Convert.ToInt32(tmpIndex[1])].BackgroundImage = ImageO;// gán hình ảnh
                        turn = 2; // set lượt chơi tiếp
                    }
                    else// nếu ô cờ thuộc người thì gán X và thiết lập hiện thị
                    {
                        chess_board_box_arr[Convert.ToInt32(tmpIndex[0]), Convert.ToInt32(tmpIndex[1])].Owner = turn;// gán người sở hữu 
                        chess_board_box_arr[Convert.ToInt32(tmpIndex[0]), Convert.ToInt32(tmpIndex[1])].BackgroundImage = ImageX;// gán hình ảnh
                        turn = 1; // set lượt chơi tiếp
                    }

                }
                if (game_mode == 2)// nếu game là chơi với máy thì sau khi load game người chơi sẽ chơi đầu
                {
                    turn = 2;
                }
            }
            catch (Exception)
            {
                // nếu có lỗi thì thông báo
                string messerror = rm.GetString("Saveerror", culture);
                MessageBox.Show(messerror);
            }
        }

        public void ChessArrayBox(int size)// tạo mảng để ghi dữ liệu ô cờ
        {
            chess_board_box_arr = new ChessBoard_Box[size, size];// tạo mảng kích thước bàn cờ là mảng ô cờ
            for (int i = 0; i < size; i++)// duyệt để tạo mới phần tử
                for (int j = 0; j < size; j++)
                {
                    chess_board_box_arr[i, j] = new ChessBoard_Box(i, j, 0);
                }
        }

        // chơi với người
        public void PVP()
        {
            //chơi với người
            game_mode = 1;
            //random lượt đi
            turn = rd.Next(1, 2);
            // nếu lượt đi là 1 thì O đi trước
            if (turn == 1)
            {
                string messeO = rm.GetString("Ogofist", culture);
                MessageBox.Show(messeO);
            }
            else// nếu lượt đi là 2 thì X đi trước
            {
                string messeX = rm.GetString("Xgofist", culture);
                MessageBox.Show(messeX);
            }
            ready = true;// sẵn sàng để chơi
            
            //khởi tạo lại stack
            sta_player_turn = new Stack<ChessBoard_Box>();
        }

        
        public void PVE()// chơi với máy
        {
            //chơi với máy
            game_mode = 2;
           
            // hỏi người chơi có muốn đi trước không
            string messeO = rm.GetString("Askgo", culture);
            string messAsk= rm.GetString("Ask", culture);
            DialogResult dialogResult = MessageBox.Show(messeO, messAsk, MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)// nếu có thì đi trước
            {
                turn = 2;
                string messeYou = rm.GetString("Yougofist", culture);
                MessageBox.Show(messeYou);
            }
            else// nếu không thì máy đi trước
            {
                turn = 1;
                string messeCom = rm.GetString("Computergofist", culture);
                MessageBox.Show(messeCom);
                ComPlay();
            }

            ready = true;// sãn sàng để chơi
            sta_player_turn = new Stack<ChessBoard_Box>();// tạo đối tượng để check vị trí cuối
            if(!isGameOver)
            {
                ComPlay();// máy đánh
            }

            
        }



        public void SetChessBox(int row, int cloumn)// gọi sự kiện click chuột cho ô cờ khi máy đánh
        {
            chess_board_box_arr[row, cloumn].Owner = turn;
            chess_board_box_arr[row, cloumn].PerformClick();
            
        }
        public void ComPlay()// máy đánh
        {
            // thiết lập điểm để tính nước đi
            int DiemMax = 0; 
            int DiemPhongNgu = 0;
            int DiemTanCong = 0;
            bool hetnuocdi = true;// hết nước
            ChessBoard_Box chess_box = new ChessBoard_Box();

            if (turn == 1)
            {
                //lượt đi đầu tiên sẽ đánh random trong khoảng chính giữa trên bàn cờ
                if (sta_player_turn.Count == 0)
                {
                    SetChessBox( rd.Next((frmMain.size / 2), (frmMain.size / 2)), rd.Next((frmMain.size / 2 ), (frmMain.size / 2 )));
                }
                else
                {
                    //thuật toán minmax tìm điểm cao nhất để đánh
                    for (int i = 0; i < frmMain.size; i++)
                    {
                        for (int j = 0; j < frmMain.size; j++)
                        {
                            //nếu nước cờ chưa có ai đánh và không bị cắt tỉa thì mới xét giá trị MinMax
                            if (chess_board_box_arr[i, j].Owner == 0 && !catTia(chess_board_box_arr[i, j]))
                            {
                                int DiemTam;// lưu điểm tạm qua mỗi lần duyệt

                                DiemTanCong = duyetTC_Ngang(i, j) + duyetTC_Doc(i, j) + duyetTC_CheoXuoi(i, j) + duyetTC_CheoNguoc(i, j);// tính điểm tấn công của ô
                                DiemPhongNgu = duyetPN_Ngang(i, j) + duyetPN_Doc(i, j) + duyetPN_CheoXuoi(i, j) + duyetPN_CheoNguoc(i, j);// tính điểm phòng ngự của ô

                                if (DiemPhongNgu > DiemTanCong)// so sanh điểm tấn công và phòng ngự để lấy điểm cao nhất
                                {
                                    DiemTam = DiemPhongNgu;
                                }
                                else
                                {
                                    DiemTam = DiemTanCong;
                                }

                                if (DiemMax < DiemTam)// nếu điểm tạm của ô đó là lơn nhất thì chọn ô đó là ô sẽ là ô sẽ đánh tiếp
                                {
                                    hetnuocdi = false;// biến check nước có thể đi khi điểm tạm âm
                                    DiemMax = DiemTam; // gán điểm của ô lớn nhất so sánh
                                    chess_box = new ChessBoard_Box(chess_board_box_arr[i, j].Row, chess_board_box_arr[i, j].Cloumn);//tạo ô cờ để đánh
                                    
                                }
                            }
                            
                        }
                    }
                    // khi hết nước đi thi ra thông báo
                    if(hetnuocdi)
                    {
                        string messeCantgo = rm.GetString("Cantgoanymore", culture);
                        MessageBox.Show(messeCantgo);
                    }
                    else
                    SetChessBox(chess_box.Row, chess_box.Cloumn);// nếu không thì đánh cờ
                }
            }
        }
        private void Btn_Click(object sender, EventArgs e)// sự kiện của nút
        {
            ChessBoard_Box oldButtonClicked = new ChessBoard_Box();// tạo một nút để lưu vị trí click sau cùng
            if(sta_buttonClicked.Count>0)// nếu đã đánh
            {
                oldButtonClicked = sta_buttonClicked.Pop(); // lấy nút đanh cuối ra.
                oldButtonClicked.BackColor = Color.White;// chuyển về màu trắng        
                sta_buttonClicked.Push(oldButtonClicked);    // sau đo push vào những ô đã đánh
            }

            ChessBoard_Box btn = sender as ChessBoard_Box; // tạo một đối tưởng để sử lý nút được truyền
            if (turn==1) 
            {
                btn.Owner = turn;//set sở hữu của nút
                btn.BackgroundImage = ImageO;// đổi ảnh nút là dấu O
                turn = 2;// đổi lượt chơi
            }
            else
            {
                btn.Owner = turn;//set sở hữu của nút
                btn.BackgroundImage = ImageX;// đổi ảnh nút là dấu X
                turn = 1;// đổi lượt chơi
            }
            btn.BackColor = Color.Yellow;// set màu cho nút mới đánh
            sta_buttonClicked.Push(btn);//đổi màu xong cho vào stack

            CheckWin();// check xem thắng hay thua
            if(isGameOver)//check xem game kết thúc thì cho có thể newgame
            {
                frmMain.NewGame();
            }
            if(game_mode==2)
            {
                ComPlay();// nếu là chơi với máy thì  máy sẽ kiểm tra đến turn mình không
            }
            
            sta_player_turn.Push(btn);
            
            
        }

        public void ChangeColorWin(Button button)//tô màu vàng  ô thắng
        {
            button.BackColor = Color.Yellow;
        }

        #region Duyệt check Win (thực hiện khi đánh xong một nước cờ)


        public bool duyetNgangPhai(int row, int cloumn, int owner)// duyệt ngang phải
        {
            if (cloumn > frmMain.size - 5)// nếu bên phải không đủ 5 nước đánh thì không duyệt
                return false;
            for (int count = 1; count <= 4; count++)// duyệt 5 nước bên phải lần lượt nếu có một ô khác người sở hữu và chưa đánh thì trả ra false
            {
                if (chess_board_box_arr[row, cloumn + count].Owner != owner || chess_board_box_arr[row, cloumn + count].Owner==0)// giữ nguyên hàng tăng dần cột
                {
                    return false;
                }

            }
            for (int count = 0; count <= 4; count++)// duyệt 5 nước bên phải lần lượt nếu cùng người sở hữu tô màu
            {
                ChangeColorWin(chess_board_box_arr[row, cloumn + count]);
            }
         
            return true;// nếu không trả ra false thì điều  kiện đúng có người chiến thắng
        }

        public bool duyetNgangTrai(int row, int cloumn, int owner)
        {
            if (cloumn < 4)// nếu bên trái không đủ 5 nước đánh thì bỏ qua
                return false;
            for (int count = 1; count <= 4; count++)// duyệt 5 nước bên phải lần lượt nếu có một ô khác người sở hữu và chưa đánh thì trả ra false
            {
                if (chess_board_box_arr[row, cloumn - count].Owner != owner || chess_board_box_arr[row, cloumn - count].Owner==0)// giữ nguyên hàng giảm dần cột
                {
                    return false;
                }

            }
            for (int count = 0; count <= 4; count++)
            {
                ChangeColorWin(chess_board_box_arr[row, cloumn - count]);
            }
    
            return true;
        }

        public bool duyetDocTren( int row, int cloumn, int owner)
        {
            if (row < 4)
                return false;
            for (int count = 1; count <= 4; count++)
            {
                if (chess_board_box_arr[row - count, cloumn].Owner != owner || chess_board_box_arr[row - count, cloumn].Owner==0)// giữ nguyên cột giảm dần hàng
                {
                    return false;
                }

            }
            for (int count = 0; count <= 4; count++)
            {
                ChangeColorWin(chess_board_box_arr[row - count, cloumn]);
            }
            return true;
        }

        public bool duyetDocDuoi(int row, int cloumn, int owner)
        {
            if (row > frmMain.size - 5)
                return false;
            for (int count = 1; count <= 4; count++)
            {
                if (chess_board_box_arr[row + count, cloumn].Owner != owner || chess_board_box_arr[row + count, cloumn].Owner==0)// giữ nguyên cột tăng dần hàng
                {
                    return false;
                }

            }
            for (int count = 0; count <= 4; count++)
            {
                ChangeColorWin(chess_board_box_arr[row + count, cloumn]);
            }
            return true;
        }

        public bool duyetCheoXuoiTren(int row, int cloumn, int owner)
        {
            if (row < 4 || cloumn < 4)
                return false;
            for (int count = 1; count <= 4; count++)
            {
                if (chess_board_box_arr[row - count, cloumn - count].Owner != owner || chess_board_box_arr[row - count, cloumn - count].Owner==0)// giảm dần cả hàng và cột
                {
                    return false;
                }

            }
            for (int count = 0; count <= 4; count++)
            {
                ChangeColorWin(chess_board_box_arr[row - count, cloumn - count]);
            }

            return true;
        }

        public bool duyetCheoXuoiDuoi(int row, int cloumn, int owner)
        {
            if (row > frmMain.size - 5 || cloumn > frmMain.size - 5)
                return false;
            for (int count = 1; count <= 4; count++)
            {
                if (chess_board_box_arr[row + count, cloumn + count].Owner != owner || chess_board_box_arr[row + count, cloumn + count].Owner==0)//tăng dần cả hàng và cột
                {
                    return false;
                }

            }
            for (int count = 0; count <= 4; count++)
            {
                ChangeColorWin(chess_board_box_arr[row + count, cloumn + count]);
            }
            return true;
        }

        public bool duyetCheoNguocDuoi(int row, int cloumn, int owner)
        {
            if (row > frmMain.size - 5 || cloumn < 4)
                return false;
            for (int count = 1; count <= 4; count++)
            {
                if (chess_board_box_arr[row + count, cloumn - count].Owner != owner || chess_board_box_arr[row + count, cloumn - count].Owner==0)//tăng hàng giảm cột
                {
                    return false;
                }

            }
            for (int count = 0; count <= 4; count++)
            {
                ChangeColorWin(chess_board_box_arr[row + count, cloumn - count]);
            }
            return true;
        }

        public bool duyetCheoNguocTren(int row, int cloumn, int owner)
        {
            if (row < 4 || cloumn > frmMain.size - 5)
                return false;
            for (int count = 1; count <= 4; count++)
            {
                if (chess_board_box_arr[row - count, cloumn + count].Owner != owner || chess_board_box_arr[row - count, cloumn + count].Owner==0)//giảm hàng tăng cột
                {
                    return false;
                }

            }
            for (int count = 0; count <= 4; count++)
            {
                ChangeColorWin(chess_board_box_arr[row - count, cloumn + count]);
            }
            return true;
        }

        #endregion

        #region Cắt tỉa Alpha betal cắt những ô cờ không thể tạo nước khỏi không gian ô cờ có thể đánh
        bool catTia(ChessBoard_Box board_Box)
        {
            //nếu cả 4 hướng đều không có nước cờ thì cắt tỉa
            if (catTiaNgang(board_Box) && catTiaDoc(board_Box) && catTiaCheoPhai(board_Box) && catTiaCheoTrai(board_Box))
                return true;

            //chạy đến đây thì 1 trong 4 hướng vẫn có nước cờ thì không được cắt tỉa
            return false;
        }

        bool catTiaNgang(ChessBoard_Box board_Box)
        {
            //duyệt bên phải
            if (board_Box.Cloumn <= frmMain.size - 5)
                for (int i = 1; i <= 4; i++)
                    if (chess_board_box_arr[board_Box.Row, board_Box.Cloumn + i].Owner != 0)//nếu có nước cờ thì không cắt tỉa
                        return false;

            //duyệt bên trái
            if (board_Box.Cloumn >= 4)
                for (int i = 1; i <= 4; i++)
                    if (chess_board_box_arr[board_Box.Row, board_Box.Cloumn - i].Owner != 0)//nếu có nước cờ thì không cắt tỉa
                        return false;

            //nếu chạy đến đây tức duyệt 2 bên đều không có nước đánh thì cắt tỉa
            return true;
        }
        bool catTiaDoc(ChessBoard_Box board_Box)
        {
            //duyệt phía giưới
            if (board_Box.Row <= frmMain.size - 5)
                for (int i = 1; i <= 4; i++)
                    if (chess_board_box_arr[board_Box.Row + i, board_Box.Cloumn].Owner != 0)//nếu có nước cờ thì không cắt tỉa
                        return false;

            //duyệt phía trên
            if (board_Box.Row >= 4)
                for (int i = 1; i <= 4; i++)
                    if (chess_board_box_arr[board_Box.Row - i, board_Box.Cloumn].Owner != 0)//nếu có nước cờ thì không cắt tỉa
                        return false;

            //nếu chạy đến đây tức duyệt 2 bên đều không có nước đánh thì cắt tỉa
            return true;
        }
        bool catTiaCheoPhai(ChessBoard_Box board_Box)
        {
            //duyệt từ trên xuống
            if (board_Box.Row <= frmMain.size - 5 && board_Box.Cloumn >= 4)
                for (int i = 1; i <= 4; i++)
                    if (chess_board_box_arr[board_Box.Row + i, board_Box.Cloumn - i].Owner != 0)//nếu có nước cờ thì không cắt tỉa
                        return false;

            //duyệt từ giưới lên
            if (board_Box.Cloumn <= frmMain.size - 5 && board_Box.Row >= 4)
                for (int i = 1; i <= 4; i++)
                    if (chess_board_box_arr[board_Box.Row - i, board_Box.Cloumn + i].Owner != 0)//nếu có nước cờ thì không cắt tỉa
                        return false;

            //nếu chạy đến đây tức duyệt 2 bên đều không có nước đánh thì cắt tỉa
            return true;
        }
        bool catTiaCheoTrai(ChessBoard_Box board_Box)
        {
            //duyệt từ trên xuống
            if (board_Box.Row <= frmMain.size - 5 && board_Box.Cloumn <= frmMain.size - 5)
                for (int i = 1; i <= 4; i++)
                    if (chess_board_box_arr[board_Box.Row + i, board_Box.Cloumn + i].Owner != 0)//nếu có nước cờ thì không cắt tỉa
                        return false;

            //duyệt từ giưới lên
            if (board_Box.Cloumn >= 4 && board_Box.Row >= 4)
                for (int i = 1; i <= 4; i++)
                    if (chess_board_box_arr[board_Box.Row - i, board_Box.Cloumn - i].Owner != 0)//nếu có nước cờ thì không cắt tỉa
                        return false;

            //nếu chạy đến đây tức duyệt 2 bên đều không có nước đánh thì cắt tỉa
            return true;
        }

        #endregion

        #region AI

        // tạo mảng điểm theo thứ tự tăng dần
        private int[] MangDiemTanCong = new int[7] { 0, 4, 25, 246, 7300, 19773, 177957 };// ưu tiên tấn công khi quân địch 2 bên trong khoảng 1-5 
        private int[] MangDiemPhongNgu = new int[7] { 0, 3, 24, 243, 2197, 20000, 200000 };// ưu tiên phòng thủ khi quân địch 2 bên trong khoảng 6-7 
        #region Tấn công
        //duyệt ngang

        // biến dùng để đếm quân địch và quân ta trên đường duyệt để tính toán phong thủ tấn công ưu tiên tấn công khi quân địch 2 bên trong khoảng 1-5 ưu tiên phòng thủ khi quân địch 2 bên trong khoảng 6-7 
        public int duyetTC_Ngang(int Row, int Cloumn)
        {
            int DiemTanCong = 0;
            int SoQuanTa = 0;
            int SoQuanDichPhai = 0;
            int SoQuanDichTrai = 0;
            int KhoangChong = 0;

            //bên phải
            for (int count = 1; count <= 4 && Cloumn < frmMain.size - 5; count++)
            {

                if (chess_board_box_arr[Row, Cloumn + count].Owner == 1)// nếu bên phải có quân ta thì thêm điểm tấn công
                {
                    if (count == 1)
                        DiemTanCong += 33;
                    // tăng sô quân và khoảng chống
                    SoQuanTa++;
                    KhoangChong++;
                }
                else
                    if (chess_board_box_arr[Row, Cloumn + count].Owner == 2)// kiểm tra số quân địch bên cạnh
                    {
                        SoQuanDichPhai++;// tăng sô quân địch và dừng kiểm tra bên phải
                        break;
                    }
                    else KhoangChong++;// nếu không có quân địch tăng khoảng trống
            }
            //bên trái tương tự như bên phải
            for (int count = 1; count <= 4 && Cloumn > 4; count++)
            {
                if (chess_board_box_arr[Row, Cloumn - count].Owner == 1)
                {
                    if (count == 1)
                        DiemTanCong += 33;

                    SoQuanTa++;
                    KhoangChong++;

                }
                else
                    if (chess_board_box_arr[Row, Cloumn - count].Owner == 2)
                    {
                        SoQuanDichTrai++;
                        break;
                    }
                    else KhoangChong++;
            }
            //bị chặn 2 đầu khoảng chống không đủ tạo thành 5 nước
            if (SoQuanDichPhai > 0 && SoQuanDichTrai > 0 && KhoangChong < 4)
            {
                return 0;
            } 
            DiemTanCong -= MangDiemPhongNgu[SoQuanDichPhai + SoQuanDichTrai];//tính điểm tấn công theo quân địch 2 bên càng nhiều quân địch càng giảm tấn công
            DiemTanCong += MangDiemTanCong[SoQuanTa];//tính thêm điểm tấn công dựa vào số quân ta
            return DiemTanCong;
        }

        //duyệt dọc
        public int duyetTC_Doc(int Row, int Cloumn)
        {
            int DiemTanCong = 0;
            int SoQuanTa = 0;
            int SoQuanDichTren = 0;
            int SoQuanDichDuoi = 0;
            int KhoangChong = 0;

            //bên trên
            for (int count = 1; count <= 4 && Row > 4; count++)
            {
                if (chess_board_box_arr[Row - count, Cloumn].Owner == 1)
                {
                    if (count == 1)
                        DiemTanCong += 33;

                    SoQuanTa++;
                    KhoangChong++;

                }
                else
                    if (chess_board_box_arr[Row - count, Cloumn].Owner == 2)
                {
                    SoQuanDichTren++;
                    break;
                }
                else KhoangChong++;
            }
            //bên dưới
            for (int count = 1; count <= 4 && Row < frmMain.size - 5; count++)
            {
                if (chess_board_box_arr[Row + count, Cloumn].Owner == 1)
                {
                    if (count == 1)
                        DiemTanCong += 33;

                    SoQuanTa++;
                    KhoangChong++;

                }
                else
                    if (chess_board_box_arr[Row + count, Cloumn].Owner == 2)
                {
                    SoQuanDichDuoi++;
                    break;
                }
                else KhoangChong++;
            }
            //bị chặn 2 đầu khoảng chống không đủ tạo thành 5 nước
            if (SoQuanDichTren > 0 && SoQuanDichDuoi > 0 && KhoangChong < 4)
            {
                return 0;
            }

            DiemTanCong -= MangDiemPhongNgu[SoQuanDichTren + SoQuanDichDuoi];
            DiemTanCong += MangDiemTanCong[SoQuanTa];
            return DiemTanCong;
        }

        //chéo xuôi
        public int duyetTC_CheoXuoi(int Row, int Cloumn)
        {
            int DiemTanCong = 1;
            int SoQuanTa = 0;
            int SoQuanDichCheoTren = 0;
            int SoQuanDichCheoDuoi = 0;
            int KhoangChong = 0;

            //bên chéo xuôi xuống
            for (int count = 1; count <= 4 && Cloumn < frmMain.size - 5 && Row < frmMain.size - 5; count++)
            {
                if (chess_board_box_arr[Row + count, Cloumn + count].Owner == 1)
                {
                    if (count == 1)
                        DiemTanCong += 33;

                    SoQuanTa++;
                    KhoangChong++;

                }
                else
                    if (chess_board_box_arr[Row + count, Cloumn + count].Owner == 2)
                    {
                        SoQuanDichCheoTren++;
                        break;
                    }
                    else KhoangChong++;
            }
            //chéo xuôi lên
            for (int count = 1; count <= 4 && Row > 4 && Cloumn > 4; count++)
            {
                if (chess_board_box_arr[Row - count, Cloumn - count].Owner == 1)
                {
                    if (count == 1)
                        DiemTanCong += 33;

                    SoQuanTa++;
                    KhoangChong++;

                }
                else
                    if (chess_board_box_arr[Row - count, Cloumn - count].Owner == 2)
                    {
                        SoQuanDichCheoDuoi++;
                        break;
                    }
                    else KhoangChong++;
            }
            //bị chặn 2 đầu khoảng chống không đủ tạo thành 5 nước
            if (SoQuanDichCheoTren > 0 && SoQuanDichCheoDuoi > 0 && KhoangChong < 4)
            {
                return 0;
            }
            //nếu không bị chặn thì tính toán điểm tấn công phòng ngự
            DiemTanCong -= MangDiemPhongNgu[SoQuanDichCheoTren + SoQuanDichCheoDuoi];
            DiemTanCong += MangDiemTanCong[SoQuanTa];
            return DiemTanCong;
        }

        //chéo ngược
        public int duyetTC_CheoNguoc(int Row, int Cloumn)
        {
            int DiemTanCong = 0;
            int SoQuanTa = 0;
            int SoQuanDichCheoTren = 0;
            int SoQuanDichCheoDuoi = 0;
            int KhoangChong = 0;

            //chéo ngược lên
            for (int count = 1; count <= 4 && Cloumn < frmMain.size - 5 && Row > 4; count++)
            {
                if (chess_board_box_arr[Row - count, Cloumn + count].Owner == 1)
                {
                    if (count == 1)
                        DiemTanCong += 33;

                    SoQuanTa++;
                    KhoangChong++;

                }
                else
                    if (chess_board_box_arr[Row - count, Cloumn + count].Owner == 2)
                {
                    SoQuanDichCheoTren++;
                    break;
                }
                else KhoangChong++;
            }
            //chéo ngược xuống
            for (int count = 1; count <= 4 && Cloumn > 4 && Row < frmMain.size - 5; count++)
            {
                if (chess_board_box_arr[Row + count, Cloumn - count].Owner == 1)
                {
                    if (count == 1)
                        DiemTanCong += 33;

                    SoQuanTa++;
                    KhoangChong++;

                }
                else
                    if (chess_board_box_arr[Row + count, Cloumn - count].Owner == 2)
                {
                    SoQuanDichCheoDuoi++;
                    break;
                }
                else KhoangChong++;
            }
            //bị chặn 2 đầu khoảng chống không đủ tạo thành 5 nước
            if (SoQuanDichCheoTren > 0 && SoQuanDichCheoDuoi > 0 && KhoangChong < 4)
            {
                return 0;
            }
            //nếu không bị chặn thì tính toán điểm tấn công phòng ngự
            DiemTanCong -= MangDiemPhongNgu[SoQuanDichCheoTren + SoQuanDichCheoDuoi];
            DiemTanCong += MangDiemTanCong[SoQuanTa];
            return DiemTanCong;
        }
        #endregion

        #region phòng ngự

        //duyệt ngang
        public int duyetPN_Ngang(int Row, int Cloumn)
        {
            int DiemPhongNgu = 0;
            int SoQuanTaTrai = 0;
            int SoQuanTaPhai = 0;
            int SoQuanDich = 0;
            int KhoangChongPhai = 0;
            int KhoangChongTrai = 0;
            bool ok = false;


            for (int count = 1; count <= 4 && Cloumn < frmMain.size - 5; count++)
            {
                if (chess_board_box_arr[Row, Cloumn + count].Owner == 2)// mỗi ô quân địch tăng điểm phòng ngự
                {
                    if (count == 1)
                        DiemPhongNgu += 9;

                    SoQuanDich++;// tăng sô quân địch
                }
                else
                    if (chess_board_box_arr[Row, Cloumn + count].Owner == 1)// nếu có quân ta giảm điểm phòng ngự(ưu tiên tấn công)
                    {
                        if (count == 4)
                            DiemPhongNgu -= 170;

                        SoQuanTaTrai++;//tăng số quân ta bên trái
                        break;
                    }
                    else
                    {
                        if (count == 1)
                            ok = true;// có ô cờ trắng 

                        KhoangChongPhai++; // tăng khoảng trống
                    }
            }

            if (SoQuanDich == 3 && KhoangChongPhai == 1 && ok)// nếu chỉ có 3 quân mà chỉ còn một ô trống thì giảm tiếp phòng ngự
                DiemPhongNgu -= 2000;

            ok = false;

            for (int count = 1; count <= 4 && Cloumn > 4; count++)
            {
                if (chess_board_box_arr[Row, Cloumn - count].Owner == 2)
                {
                    if (count == 1)
                        DiemPhongNgu += 9;

                    SoQuanDich++;
                }
                else
                    if (chess_board_box_arr[Row, Cloumn - count].Owner == 1)
                    {
                        if (count == 4)
                            DiemPhongNgu -= 170;

                        SoQuanTaPhai++;// tăng quân ta bên phải
                        break;
                    }
                    else
                    {
                        if (count == 1)
                            ok = true;// có ô cờ trắng 

                        KhoangChongTrai++;
                    }
            }

            if (SoQuanDich == 3 && KhoangChongTrai == 1 && ok)
                DiemPhongNgu -= 2000;

            if (SoQuanTaPhai > 0 && SoQuanTaTrai > 0 && (KhoangChongTrai + KhoangChongPhai + SoQuanDich) < 4)
                return 0;

            DiemPhongNgu -= MangDiemTanCong[SoQuanTaPhai + SoQuanTaPhai]; // ưu tiên tấn công khi quân địch 2 bên trong khoảng 1-5 
            DiemPhongNgu += MangDiemPhongNgu[SoQuanDich];//ưu tiên phòng thủ khi quân địch xung quanh đường duyệt 6-7

            return DiemPhongNgu;
        }

        //duyệt dọc
        public int duyetPN_Doc(int Row, int Cloumn)
        {
            int DiemPhongNgu = 0;
            int SoQuanTaTrai = 0;
            int SoQuanTaPhai = 0;
            int SoQuanDich = 0;
            int KhoangChongTren = 0;
            int KhoangChongDuoi = 0;
            bool ok = false;

            for (int count = 1; count <= 4 && Row > 4; count++)
            {
                if (chess_board_box_arr[Row - count, Cloumn].Owner == 2)
                {
                    if (count == 1)
                        DiemPhongNgu += 9;

                    SoQuanDich++;

                }
                else
                    if (chess_board_box_arr[Row - count, Cloumn].Owner == 1)
                    {
                        if (count == 4)
                            DiemPhongNgu -= 170;

                        SoQuanTaPhai++;
                        break;
                    }
                    else
                    {
                        if (count == 1)
                            ok = true;// có ô cờ trắng 

                        KhoangChongTren++;
                    }
            }

            if (SoQuanDich == 3 && KhoangChongTren == 1 && ok)
                DiemPhongNgu -= 2000;

            ok = false;
            for (int count = 1; count <= 4 && Row < frmMain.size - 5; count++)
            {
                
                if (chess_board_box_arr[Row + count, Cloumn].Owner == 2)//gặp quân địch
                {
                    if (count == 1)
                        DiemPhongNgu += 9;

                    SoQuanDich++;
                }
                else
                    if (chess_board_box_arr[Row + count, Cloumn].Owner == 1)
                    {
                        if (count == 4)
                            DiemPhongNgu -= 170;

                        SoQuanTaTrai++;
                        break;
                    }
                    else
                    {
                        if (count == 1)
                            ok = true;// có ô cờ trắng 

                        KhoangChongDuoi++;
                    }
            }

            if (SoQuanDich == 3 && KhoangChongDuoi == 1 && ok)
                DiemPhongNgu -= 2000;

            if (SoQuanTaPhai > 0 && SoQuanTaTrai > 0 && (KhoangChongTren + KhoangChongDuoi + SoQuanDich) < 4)
                return 0;

            DiemPhongNgu -= MangDiemTanCong[SoQuanTaTrai + SoQuanTaPhai];
            DiemPhongNgu += MangDiemPhongNgu[SoQuanDich];
            return DiemPhongNgu;
        }

        //chéo xuôi
        public int duyetPN_CheoXuoi(int Row, int Cloumn)
        {
            int DiemPhongNgu = 0;
            int SoQuanTaTrai = 0;
            int SoQuanTaPhai = 0;
            int SoQuanDich = 0;
            int KhoangChongTren = 0;
            int KhoangChongDuoi = 0;
            bool ok = false;

            
            for (int count = 1; count <= 4 && Row < frmMain.size - 5 && Cloumn < frmMain.size - 5; count++)
            {
                if (chess_board_box_arr[Row + count, Cloumn + count].Owner == 2)
                {
                    if (count == 1)
                        DiemPhongNgu += 9;

                    SoQuanDich++;
                }
                else
                    if (chess_board_box_arr[Row + count, Cloumn + count].Owner == 1)
                    {
                        if (count == 4)
                            DiemPhongNgu -= 170;

                        SoQuanTaPhai++;
                        break;
                    }
                    else
                    {
                        if (count == 1)
                            ok = true;// có ô cờ trắng 

                        KhoangChongTren++;
                    }
            }

            if (SoQuanDich == 3 && KhoangChongTren == 1 && ok)
                DiemPhongNgu -= 2000;

            ok = false;
            
            for (int count = 1; count <= 4 && Row > 4 && Cloumn > 4; count++)
            {
                if (chess_board_box_arr[Row - count, Cloumn - count].Owner == 2)
                {
                    if (count == 1)
                        DiemPhongNgu += 9;

                    SoQuanDich++;
                }
                else
                    if (chess_board_box_arr[Row - count, Cloumn - count].Owner == 1)
                    {
                        if (count == 4)
                            DiemPhongNgu -= 170;

                        SoQuanTaTrai++;
                        break;
                    }
                    else
                    {
                        if (count == 1)
                            ok = true;// có ô cờ trắng 

                        KhoangChongDuoi++;
                    }
            }

            if (SoQuanDich == 3 && KhoangChongDuoi == 1 && ok)
                DiemPhongNgu -= 2000;

            if (SoQuanTaPhai > 0 && SoQuanTaTrai > 0 && (KhoangChongTren + KhoangChongDuoi + SoQuanDich) < 4)
                return 0;

            DiemPhongNgu -= MangDiemTanCong[SoQuanTaPhai + SoQuanTaTrai];
            DiemPhongNgu += MangDiemPhongNgu[SoQuanDich];

            return DiemPhongNgu;
        }

        //chéo ngược
        public int duyetPN_CheoNguoc(int Row, int Cloumn)
        {
            int DiemPhongNgu = 0;
            int SoQuanTaTrai = 0;
            int SoQuanTaPhai = 0;
            int SoQuanDich = 0;
            int KhoangChongTren = 0;
            int KhoangChongDuoi = 0;
            bool ok = false;

            
            for (int count = 1; count <= 4 && Row > 4 && Cloumn < frmMain.size - 5; count++)
            {

                if (chess_board_box_arr[Row - count, Cloumn + count].Owner == 2)
                {
                    if (count == 1)
                        DiemPhongNgu += 9;

                    SoQuanDich++;
                }
                else
                    if (chess_board_box_arr[Row - count, Cloumn + count].Owner == 1)
                    {
                        if (count == 4)
                            DiemPhongNgu -= 170;

                        SoQuanTaPhai++;
                        break;
                    }
                    else
                    {
                        if (count == 1)
                            ok = true;// có ô cờ trắng 

                        KhoangChongTren++;
                    }
            }


            if (SoQuanDich == 3 && KhoangChongTren == 1 && ok)
                DiemPhongNgu -= 2000;

            ok = false;

         
            for (int count = 1; count <= 4 && Row < frmMain.size - 5 && Cloumn > 4; count++)
            {
                if (chess_board_box_arr[Row + count, Cloumn - count].Owner == 2)
                {
                    if (count == 1)
                        DiemPhongNgu += 9;

                    SoQuanDich++;
                }
                else
                    if (chess_board_box_arr[Row + count, Cloumn - count].Owner == 1)
                    {
                        if (count == 4)
                            DiemPhongNgu -= 170;

                        SoQuanTaTrai++;
                        break;
                    }
                    else
                    {
                        if (count == 1)
                            ok = true;// có ô cờ trắng 

                        KhoangChongDuoi++;
                    }
            }

            if (SoQuanDich == 3 && KhoangChongDuoi == 1 && ok)
                DiemPhongNgu -= 2000;

            if (SoQuanTaPhai > 0 && SoQuanTaTrai > 0 && (KhoangChongTren + KhoangChongDuoi + SoQuanDich) < 4)
                return 0;

            DiemPhongNgu -= MangDiemTanCong[SoQuanTaTrai + SoQuanTaPhai];
            DiemPhongNgu += MangDiemPhongNgu[SoQuanDich];

            return DiemPhongNgu;
        }

        #endregion

        #endregion
        public bool CheckWin()// kiểm tra chiến thắng
        {
            if (sta_buttonClicked.Count != 0)// khi đã có lượt chơi
            {
                foreach (ChessBoard_Box chessbox in sta_player_turn)
                {
                    //duyệt theo 8 hướng mỗi quân cờ vừa đánh
                    if (duyetNgangPhai( chessbox.Row, chessbox.Cloumn, chessbox.Owner) || duyetNgangTrai( chessbox.Row, chessbox.Cloumn, chessbox.Owner)
                        || duyetDocTren( chessbox.Row, chessbox.Cloumn, chessbox.Owner) || duyetDocDuoi( chessbox.Row, chessbox.Cloumn, chessbox.Owner)
                        || duyetCheoXuoiTren( chessbox.Row, chessbox.Cloumn, chessbox.Owner) || duyetCheoXuoiDuoi( chessbox.Row, chessbox.Cloumn, chessbox.Owner)
                        || duyetCheoNguocTren( chessbox.Row, chessbox.Cloumn, chessbox.Owner) || duyetCheoNguocDuoi( chessbox.Row, chessbox.Cloumn, chessbox.Owner))
                    {
                        GameOver(chessbox);// nạp ô thắng để báo kết quả
                        isGameOver = true;// game đã kết thúc
                        foreach (ChessBoard_Box box in chess_board_box_arr)// chặn không cho người dùng đánh nữa
                        {
                            box.Enabled = false;
                        }
                        return true;
                    }
                }
            }

            return false;
        }

        private void GameOver(ChessBoard_Box chessbox) // thông báo chiến thắng
        {
            //chơi với người
            if (game_mode == 1)
            {
                if (chessbox.Owner == 1)
                {
                    string messOwon = rm.GetString("Owon", culture);

                    MessageBox.Show(messOwon);
                }
                else
                {
                    string messXwon = rm.GetString("Xwon", culture);
                    MessageBox.Show(messXwon);
                }
            }
            else//chơi với máy
            {
                if (chessbox.Owner == 1)
                {
                    string messComwon = rm.GetString("Computerwon", culture);
                    MessageBox.Show(messComwon);
                }
                else
                {
                    string messYouwon = rm.GetString("Youwon", culture);
                    MessageBox.Show(messYouwon);
                }
            }

            ready = false;
            isGameOver = true;
        }
        #endregion
    }
}
