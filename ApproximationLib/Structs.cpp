//#include "pch.h"
#include "Structs.h"

#include <malloc.h>

void FreeMatrix(MatrixMxN& matrix) {
    for (int i = 0; i < matrix.rowCount; i++)
        free(matrix.matrix[i]);
    free(matrix.matrix);
}

void FreeEqualitySolution(EqualitySolution& solution)
{
    free(solution.variables);
}