import React, { useState } from 'react';
import parse from 'html-react-parser';
import './App.css';

const Chessboard = ({ queens }) => {
  const boardSize = queens.length;

  const renderSquare = (row, col, isQueen) => {
    const squareColor = (row + col) % 2 === 0 ? 'white' : 'black';
    const queenClassName = isQueen ? 'red-queen' : '';

    return (
        <div
            key={`${row}-${col}`}
            className={`square ${queenClassName}`}
            style={{ backgroundColor: squareColor }}
        >
            {isQueen && '♕'}
        </div>
    );
  };

  const renderRow = (rowIndex) => {
    const row = [];
    for (let col = 0; col < boardSize; col++) {
      const isQueen = queens[rowIndex] === col;
      row.push(renderSquare(rowIndex, col, isQueen));
    }
    return row;
  };

  const renderBoard = () => {
    const board = [];
    for (let row = 0; row < boardSize; row++) {
      board.push(
        <div key={row} style={{ display: 'flex' }}>
          {renderRow(row)}
        </div>
      );
    }
    return board;
  };

  return <div>{renderBoard()}</div>;
};

const App = () => {
  const [queens, setQueens] = useState([]);
  const [numQueens, setNumQueens] = useState('');
  let  [result, setRs]  = useState('');
  const solveNQueens = async () => {
    try {
      const response = await fetch("https://localhost:44393?num=" + numQueens)  
      .then(response => response.json())
      .then(data => { 
        setRs(data.result);
        console.log(data.result); 
      });
    } catch (error) {
      console.error('Error:', error);
    }
  };

  return (
    <div className="App">
      <h3>
        Nhập số lượng con Hậu trên bàn cờ:
        <input
          type="number"
          placeholder="Nhập số lượng Hậu"
          value={numQueens}
          onChange={(e) => setNumQueens(e.target.value)}
        />
        <button onClick={solveNQueens}>Giải</button>
      </h3>

      <h1>N-Queens Chessboard</h1>
      <div id="tableMap">{ parse(result) }</div>
      <Chessboard queens={queens} />
    </div>
  );
};

export default App;