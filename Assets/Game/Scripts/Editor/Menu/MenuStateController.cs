using Game.Boxes;
using Game.Collectable;
using Game.Editor;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System.Linq;

public class MenuStateController
{
    private EditorMenuState currentState = EditorMenuState.Unknown;
    private readonly List<(IMenuState handler, EditorMenuState state)> stateHandlers;
    private readonly Dictionary<EditorMenuTypes, HashSet<EditorMenuState>> menuStateMap;

    public MenuStateController()
    {
        stateHandlers = new List<(IMenuState, EditorMenuState)>
        {
            //Box Menu
            CreateTypeHandler<Box>(EditorMenuState.BoxSelected),
            CreateNameHandler(EditorConst.BOX_CREATION_MENU, EditorMenuState.AllBoxesSelected),
            CreateTypeHandler<BoxContent>(EditorMenuState.BoxContentSelected),
            //Reward Menu
            CreateNameHandler(EditorConst.REWARD_CREATION_MENU, EditorMenuState.AllRewards),
            CreateTypeHandler<AbstractReward>(EditorMenuState.RewardSelected),
            //item menu
            CreateNameHandler(EditorConst.ITEM_CREATION_MENU, EditorMenuState.ItemSelected),
            CreateTypeHandler<WheelItem>(EditorMenuState.ItemSelected)
        };

        menuStateMap = new Dictionary<EditorMenuTypes, HashSet<EditorMenuState>>
        {
            {
                EditorMenuTypes.BoxCreationMenu,
                new HashSet<EditorMenuState>
                {
                    EditorMenuState.AllBoxesSelected,
                    EditorMenuState.BoxSelected,
                    EditorMenuState.BoxContentSelected,
                    EditorMenuState.Unknown
                }
            },
            {
                EditorMenuTypes.RewardCreationMenu,
                new HashSet<EditorMenuState>
                {
                    EditorMenuState.AllRewards,
                    EditorMenuState.RewardSelected,
                    EditorMenuState.Unknown
                }
            },
            {
                EditorMenuTypes.ItemCreationMenu,
                new HashSet<EditorMenuState>
                {
                    EditorMenuState.ItemSelected,
                    EditorMenuState.Unknown
                }
            }
        };
    }
    public void UpdateState(OdinMenuItem currentMenu)
    {
        if (currentMenu == null)
        {
            currentState = EditorMenuState.Unknown;
            return;
        }

        var matchingHandler = stateHandlers.FirstOrDefault(x => x.handler.CanHandle(currentMenu));
        currentState = matchingHandler.state;
    }

    public bool IsMenuStateUnknown() => currentState == EditorMenuState.Unknown;

    public bool IsInMenu(EditorMenuTypes menuType)
    {
        return menuStateMap.TryGetValue(menuType, out var validStates) &&
               validStates.Contains(currentState);
    }

    private (IMenuState, EditorMenuState) CreateTypeHandler<T>(EditorMenuState state) where T : class
        => (new TypeMenuState<T>(), state);

    private (IMenuState, EditorMenuState) CreateNameHandler(string name, EditorMenuState state)
        => (new NameMenuState(name), state);

    public EditorMenuState GetCurrentState() => currentState;

    private class TypeMenuState<T> : IMenuState where T : class
    {
        public bool CanHandle(OdinMenuItem menu) => menu.Value is T;
    }

    private class NameMenuState : IMenuState
    {
        private readonly string expectedName;

        public NameMenuState(string name)
        {
            expectedName = name;
        }

        public bool CanHandle(OdinMenuItem menu) => menu.Name == expectedName;
    }
}