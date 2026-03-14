//#include "pch.h"
#include "Jem.h"
#include "Structs.h"

#include <malloc.h>
#include <set>


void DoJemStep(MatrixMxN& matrix, int row, int column);

Array SolveMatrix(MatrixMxN& matrix)
{
    int length = std::min(matrix.columnCount, matrix.rowCount);
    std::set<double> usedColumns = std::set<double>();

    for (int i = 0; i < length; i++) {
        int column = 0;
        for (int j = 0; j < length; j++) {
            if (usedColumns.find(j) != usedColumns.end() || matrix.matrix[i][j] == 0)
                continue;

            column = j;
            usedColumns.insert(j);
            break;
        }

        DoJemStep(matrix, i, column);
    }

    Array solution;
    solution.variables = (double*)malloc(sizeof(double) * length);
    solution.length = length;

    for (int i = 0; i < length; i++)
        solution.variables[i] = matrix.matrix[i][matrix.columnCount - 1];

    return solution;
}

void DoJemStep(MatrixMxN& matrix, int row, int column)
{
    if (!matrix.matrix[row][column])
        return;

    if (matrix.matrix[row][column] != 1) {
        double divisor = matrix.matrix[row][column];
        for (int i = 0; i < matrix.columnCount; i++)
            matrix.matrix[row][i] /= divisor;
    }

    for (int i = 0; i < matrix.rowCount; i++) {
        if (i == row || matrix.matrix[i][column] == 0)
            continue;

        double multiplier = matrix.matrix[i][column];

        for (int j = 0; j < matrix.columnCount; j++)
            matrix.matrix[i][j] -= multiplier * matrix.matrix[row][j];
    }
}