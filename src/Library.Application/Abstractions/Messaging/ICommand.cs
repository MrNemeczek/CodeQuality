namespace Library.Application.Abstractions.Messaging;

public interface ICommand
{
}

public interface ICommand<out TResult>
{
}
