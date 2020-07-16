using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Caro.Business.Entities
{
    class ChessBoard_Box:Button
    {
        //tạo chiều rộng và chiều cao ô cờ
        public static int box_width ;
        public static int box_height ;
        // ví trí ô cờ
        private int row; 
        private int cloumn;
        // ô cờ đã được ai đánh chưa = 0 khi chưa có người đánh
        private int owner;

        // phương thức truy cập thiết lập giá trị
        public int Row { get => row; set => row = value; }
        public int Cloumn { get => cloumn; set => cloumn = value; }
        public int Owner { get => owner; set => owner = value; }

        // khởi tạo không tham số
        public ChessBoard_Box() { }

        //khởi tạo có 3 tham số và gán cho các thuộc tính của đối tượng
        public ChessBoard_Box(int row, int cloumn, int owner)
        {
            this.row = row;
            this.cloumn = cloumn;
            this.owner = owner;
        }

        //khởi tạo có 2 tham số và gán cho các thuộc tính của đối tượng
        public ChessBoard_Box(int row, int cloumn)
        {
            this.row = row;
            this.cloumn = cloumn;
        }
    }
}
