using System.Collections.Generic;

public class BoardState(List<Depot> _depots, List<Foundation> _foundations, Foundation _drawPile, Foundation _discardPile)
{

    private List<Depot> depots = _depots;
    private List<Foundation> foundations = _foundations;
    private Foundation drawPile = _drawPile;
    private Foundation discardPile = _discardPile;

}