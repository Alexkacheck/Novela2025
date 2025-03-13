using System;
using System.Collections.Generic;

namespace Scripts.Infrastructure.StateMachine
{
    public class BaseStateMachine : IStateMachine 
    {
        private Dictionary<Type, IExitableState> _states;
        private IExitableState _activeState;
        private IFixedUpdatableState _fixedUpdatableState;
        private IUpdatableState _updatableState;

        public IExitableState ActiveState => _activeState;

        public void Enter<TState>() where TState : class, IState
        {
            IState state = ChangeState<TState>();
            state.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
        {
            TState state = ChangeState<TState>();
            state.Enter(payload);
        }

        public void AddState(Type type, IExitableState state)
        {
            _states ??= new Dictionary<Type, IExitableState>();
            _states[type] = state;
        }

        public void FixedUpdateState()
        {
            _fixedUpdatableState?.FixedUpdate();
        }
        
        public void UpdateState()
        {
            _updatableState?.Update();
        }

        private TState ChangeState<TState>() where TState : class, IExitableState
        {
            _activeState?.Exit();

            TState state = GetState<TState>();

            _fixedUpdatableState = state as IFixedUpdatableState;
            _updatableState = state as IUpdatableState;
            _activeState = state;

            return state;
        }

        private TState GetState<TState>() where TState : class, IExitableState 
            => _states[typeof(TState)] as TState;
    }
}