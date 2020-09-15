using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CW_A_Simplistic_TCP_Finite_State_Machine
{
    public enum Event
    {
        APP_PASSIVE_OPEN,
        APP_ACTIVE_OPEN,
        APP_SEND,
        APP_CLOSE,
        APP_TIMEOUT,
        RCV_SYN,
        RCV_ACK,
        RCV_SYN_ACK,
        RCV_FIN,
        RCV_FIN_ACK,
        EVENT
    }
    public enum StateValue
    {
        CLOSED,
        LISTEN,
        SYN_SENT,
        SYN_RCVD,
        ESTABLISHED,
        CLOSE_WAIT,
        LAST_ACK,
        FIN_WAIT_1,
        FIN_WAIT_2,
        CLOSING,
        TIME_WAIT,
        NEW_STATE
    }
    public class Solution
    {
        public Dictionary<string, Event> eventsMap = new Dictionary<string, Event>()
        {
            ["APP_PASSIVE_OPEN"] = Event.APP_PASSIVE_OPEN,
            ["APP_ACTIVE_OPEN"] = Event.APP_ACTIVE_OPEN,
            ["APP_SEND"] = Event.APP_SEND,
            ["APP_CLOSE"] = Event.APP_CLOSE,
            ["APP_TIMEOUT"] = Event.APP_TIMEOUT,
            ["RCV_SYN"] = Event.RCV_SYN,
            ["RCV_ACK"] = Event.RCV_ACK,
            ["RCV_SYN_ACK"] = Event.RCV_SYN_ACK,
            ["RCV_FIN"] = Event.RCV_FIN,
            ["RCV_FIN_ACK"] = Event.RCV_FIN_ACK
        };

        public Dictionary<StateValue, string> stateMap = new Dictionary<StateValue, string>
        {
            [StateValue.CLOSED] = "CLOSED",
            [StateValue.LISTEN] = "LISTEN",
            [StateValue.SYN_SENT] = "SYN_SENT",
            [StateValue.SYN_RCVD] = "SYN_RCVD",
            [StateValue.ESTABLISHED] = "ESTABLISHED",
            [StateValue.CLOSE_WAIT] = "CLOSE_WAIT",
            [StateValue.LAST_ACK] = "LAST_ACK",
            [StateValue.FIN_WAIT_1] = "FIN_WAIT_1",
            [StateValue.FIN_WAIT_2] = "FIN_WAIT_2",
            [StateValue.CLOSING] = "CLOSING",
            [StateValue.TIME_WAIT] = "TIME_WAIT",
        };

        public Solution()
        {
            this.machineFactory = new MachineFactory();
        }

        private MachineFactory machineFactory;

        public string Receiving(params string[] events)
        {
            var currentState = machineFactory.CreateASimplisticTCPFiniteState();

            var result = "";
            foreach (var e in events)
            {
                var receivedEvent = eventsMap[e];
                var nextState = currentState.GetNext(receivedEvent);
                if (nextState == null)
                {
                    result = "";
                    break;
                }
                else
                    result = stateMap[nextState.Value];
                currentState = nextState;
            }

            if (string.IsNullOrEmpty(result))
                return "ERROR";

            return result;
        }
    }

    public class State
    {

        private Dictionary<Event, State> NextSteps = new Dictionary<Event, State>();

        public StateValue Value { get; }

        public State(StateValue value)
        {
            Value = value;
        }

        public void SetNext(Event key, State state)
        {
            if (!NextSteps.ContainsKey(key))
                NextSteps.TryAdd(key, state);
        }

        internal State GetNext(Event receivedEvent)
        {
            NextSteps.TryGetValue(receivedEvent, out State result);

            return result;
        }
    }

    public class MachineFactory
    {
        public State CreateASimplisticTCPFiniteState()
        {
            var state = new State(StateValue.NEW_STATE);

            var closedState = new State(StateValue.CLOSED);
            var listenState = new State(StateValue.LISTEN);
            var synRcvdState = new State(StateValue.SYN_RCVD);
            var synSentState = new State(StateValue.SYN_SENT);
            var establishedState = new State(StateValue.ESTABLISHED);
            var finWait1State = new State(StateValue.FIN_WAIT_1);
            var closingState = new State(StateValue.CLOSING);
            var finWait2State = new State(StateValue.FIN_WAIT_2);
            var timeWaitState = new State(StateValue.TIME_WAIT);
            var closeWaitState = new State(StateValue.CLOSE_WAIT);
            var lastAckState = new State(StateValue.LAST_ACK);

            state.SetNext(Event.EVENT, closedState);

            closedState.SetNext(Event.APP_PASSIVE_OPEN, listenState);
            closedState.SetNext(Event.APP_ACTIVE_OPEN, synSentState);
            listenState.SetNext(Event.RCV_SYN, synRcvdState);
            listenState.SetNext(Event.APP_SEND, synSentState);
            listenState.SetNext(Event.APP_CLOSE, closedState);
            synRcvdState.SetNext(Event.APP_CLOSE, finWait1State);
            synRcvdState.SetNext(Event.RCV_ACK, establishedState);
            synSentState.SetNext(Event.RCV_SYN, synRcvdState);
            synSentState.SetNext(Event.RCV_SYN_ACK, establishedState);
            synSentState.SetNext(Event.APP_CLOSE, closedState);
            establishedState.SetNext(Event.APP_CLOSE, finWait1State);
            establishedState.SetNext(Event.RCV_FIN, closeWaitState);
            finWait1State.SetNext(Event.RCV_FIN, closingState);
            finWait1State.SetNext(Event.RCV_FIN_ACK, timeWaitState);
            finWait1State.SetNext(Event.RCV_ACK, finWait2State);
            closingState.SetNext(Event.RCV_ACK, timeWaitState);
            finWait2State.SetNext(Event.RCV_FIN, timeWaitState);
            timeWaitState.SetNext(Event.APP_TIMEOUT, closedState);
            closeWaitState.SetNext(Event.APP_CLOSE, lastAckState);
            lastAckState.SetNext(Event.RCV_ACK, closedState);

            return closedState;
        }
    }

}
