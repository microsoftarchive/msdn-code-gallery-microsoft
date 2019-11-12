function insert(item, user, request) {
     resultsItem = item;
     lbTable = tables.getTable('Leaderboard');

     request.execute({
          success: function() {
                request.respond();
                updateLeaderboard(item, user);
          }
     });
}

var resultsItem, lbTable;

function updateLeaderboard(item, user) {
     lbTable.where({ playerName: item.playerName }).read({
          success: updateScore,
          error: errorHandler
     })
}

function updateScore(results) {
     if (results.length > 0) {
          lbTable.update({
                id: results[0].id,
                playerName: resultsItem.playerName,
                score: results[0].score + (resultsItem.hits * 100)
          }, {
                success: updatePosition,
                error: errorHandler
          })

          console.log("Score updated for player " + resultsItem.playerName + ".");
     } else {
          lbTable.insert({ 
                playerName: resultsItem.playerName,
                score: resultsItem.hits * 100,
                position: 1
          }, {
                success: updatePosition,
                error: errorHandler
          })

          console.log("Leaderboard item added for player " + resultsItem.playerName + ".");
     }
}

function updatePosition() {
     var sql = 
          "With cte As " +
          "(SELECT score,position, " +
          "ROW_NUMBER() OVER (ORDER BY score DESC) AS newposition " +
          "FROM Leaderboard) " +
          "UPDATE cte SET position = newposition";
     mssql.query(sql, {
          success: function(results) {
                console.log("Leaderboard positions updated.");

                var resultsTable = tables.getTable('Result');
                resultsTable.update({
                     id: resultsItem.id,
                     leaderboardUpdated: true
                })
          },
          error: function(error) {
                console.error(error);
          }
     });
}

function errorHandler(error){
     console.error("An error occurred trying to update leaderboard info for player " + 
          resultsItem.playerName + ".");
}