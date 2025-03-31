using System;
using System.Collections.Generic;

public class StateManager()
{
    List<BoardState> boardStateHistory = new();

    public void saveNewBoardState(List<Depot> _depots, List<Foundation> _foundations, Foundation _drawPile, Foundation _discardPile)
    {
        boardStateHistory.Add(new BoardState(_depots, _foundations, _drawPile, _discardPile));
        Console.WriteLine("Saved board state");
    }

    public void restoreLastBoardState()
    {
        
    }

    public void resetStateHistory()
    {
        boardStateHistory.Clear();
        Console.WriteLine("Resetting board state history");
    }

}