using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyTodoLog
{
    public partial class TaskModel
    {
        // タスクオブジェクトの定義、タスクの取得、追加、状態管理、削除を行うクラス
        public int Id { get; set; }
        public string Title { get; set; } = ""; // コントロール操作時に落ちやすいためnullを避けて設計します
        public string? Memo { get; set; }
        public bool IsCompleted { get; set; }

        public static List<TaskModel> GetAllTasks(string connectionString)
        {
            var tasks = new List<TaskModel>();

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Title, Memo, IsCompleted FROM Tasks";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var task = new TaskModel
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Memo = reader.IsDBNull(2) ? null : reader.GetString(2),
                    IsCompleted = reader.GetInt32(3) == 1
                };

                tasks.Add(task);
            }
            return tasks;
        }

        public static int Insert(string connectionString, string title, string? memo)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException
                    ("タスクタイトルは必須です！\n今日やりたい学習を書きましょう！", nameof(title));
            
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                                INSERT INTO Tasks (Title, Memo, IsCompleted)
                                VALUES ($title, $memo, 0);
                                SELECT last_insert_rowid();
                                ";

            command.Parameters.AddWithValue("$title", title);
            command.Parameters.AddWithValue("$memo", (object?)memo ?? DBNull.Value);

            long newId = (long)command.ExecuteScalar()!;
            return (int)newId;
        }

        public static void UpdateIsCompleted(string connectionString, int id, bool isCompleted)
        {
            // タスクの完了/未完了状態の変更を適用する
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                            UPDATE Tasks
                            SET IsCompleted = $isCompleted
                            WHERE Id = $Id;
                            ";
            command.Parameters.AddWithValue("$Id", id);
            command.Parameters.AddWithValue("$isCompleted", isCompleted ? 1 : 0);

            command.ExecuteNonQuery();
        }

        public static int DeleteCompleted(string connectionString)
        {
            // 完了済み状態のタスクオブジェクトを削除する
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                            DELETE FROM Tasks
                            WHERE IsCompleted = 1;
                            ";

            return command.ExecuteNonQuery();
        }

        public static void Update(string connectionString, int id, string title, string? memo)
        {
            //タスク名の存在を確認後、タイトルとメモ欄の変更をDBに適用する
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("タスク名は必須です", nameof(title));

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                            UPDATE Tasks
                            SET Title = $title,
                            Memo = $memo
                            WHERE Id = $id;
                            ";

            command.Parameters.AddWithValue("$id", id);
            command.Parameters.AddWithValue($"title", title);
            command.Parameters.AddWithValue($"memo", (object?)memo ?? DBNull.Value);

            command.ExecuteNonQuery();
        }
    }
}
