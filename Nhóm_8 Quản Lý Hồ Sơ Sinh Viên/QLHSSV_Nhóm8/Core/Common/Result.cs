using System;

namespace QLHSSV_Nhóm8.Core.Common
{
    /// <summary>
    /// Đại diện cho kết quả thực thi của một tác vụ
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string ErrorMessage { get; }

        protected Result(bool isSuccess, string errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage ?? string.Empty;
        }

        public static Result Success() => new(true, string.Empty);
        public static Result Failure(string errorMessage) => new(false, errorMessage);
    }

    /// <summary>
    /// Kết quả trả về kèm dữ liệu strongly-typed
    /// </summary>
    public class Result<T> : Result
    {
        public T? Data { get; }

        protected Result(bool isSuccess, T? data, string errorMessage) 
            : base(isSuccess, errorMessage)
        {
            Data = data;
        }

        public static Result<T> Success(T data) => new(true, data, string.Empty);
        public new static Result<T> Failure(string errorMessage) => new(false, default, errorMessage);
    }
}

