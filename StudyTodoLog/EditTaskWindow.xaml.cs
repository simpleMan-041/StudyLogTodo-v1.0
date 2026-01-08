using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StudyTodoLog
{

    public partial class EditTaskWindow : Window
    {
        private readonly string _connectionString;
        private readonly int _taskId;

        public EditTaskWindow(string connectionString, TaskModel task)
        {
            InitializeComponent();

            _connectionString = connectionString;
            _taskId = task.Id;

            TitleTextBox.Text = task.Title;
            MemoTextBox.Text = task.Memo;

            TitleTextBox.Focus();
            TitleTextBox.SelectAll();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // タスク名の存在を確認し、対応するタスクオブジェクトを更新
            try
            {
                string title = TitleTextBox.Text.Trim();
                string? memo = string.IsNullOrWhiteSpace(MemoTextBox.Text) ? null : MemoTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(title))
                {
                    MessageBox.Show("タスク名は必須です", "入力エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                TaskModel.Update(_connectionString, _taskId, title, memo);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("更新に失敗しました\n\n" + ex.Message, "Update Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
