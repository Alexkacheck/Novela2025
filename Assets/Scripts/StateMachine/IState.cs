namespace Scripts.Infrastructure.StateMachine
{
    public interface IExitableState
    {
        void Exit();
    }

    public interface IState : IExitableState
    {
        void Enter();
    }

    public interface IPayloadedState<TPayload> : IExitableState
    {
        void Enter(TPayload payload);
    }

    public interface IFixedUpdatableState
    {
        void FixedUpdate();
    }

    public interface IUpdatableState
    {
        void Update();
    }
}