using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Constants
{
    public class AppConstants
    {
        // Console 
        public const int CONSOLE_WIDTH = 150;
        public const int CONSOLE_HEIGHT = 40;

        // Validation
        public const int BOOK_NAME_LENGTH = 60;
        public const int ISBN_LENGTH = 13;
        public const int CNP_LENGTH = 13;

        public const int MAX_LOAN_DAYS = 14;
        public const double LOAN_PENALTY = 0.01;

        public const long ROUTINE_INTERVAL = 1000 * 60 * 60 * 24; // milliseconds 
    }
}
