Spades

This is a command-line game of Spades.  It's a single-player game, and bidding and card selection are decided using 
a variation on Information Set Monte Carlo Tree Search.  

I chose this algorithm for the computer players because it is well-suited to a game of imperfect information such as 
Spades.  Unlike games like chess, in Spades, not all information about the game state is available to each player.  However, every player knows which cards have been played, which ones are in that player's hand, and which bids were 
cast.  

When it's time for a computer player to make a decision, at each node of the decision tree, in a SimGame object, 
the other player's hands are redealt according to the constraints laid out by the information set.  A node is 
selected from the tree via the UCB1 function, and we simulate the playout of the entire game from the current 
game state at the chosen node, based on the events which have preceded the current state.  The outcome of the trial
is recorded at the node, and the tree is sampled until a time out is reached.  

My implementation differs from other examples of ISMCTS in that I create a tree that is only one level deep.  This is 
a modification made in the interest of reducing memory cost.  Since the number of possible children of any given node
is so large, the node density below a single node to trace through all possible permutations of cards in a trick is
enormous.  Therefore, I opt to sample each node (corresponding to a card available to the player making the decision)
a large number of times, each time rearranging the allocation of the other players' cards.  This has the effect of 
"flattening" the search space while the algorithm still checks through a large number of the possible trick permutations. 

Features I'm continuing to work on:
- Nil bids
- A web-based UI
