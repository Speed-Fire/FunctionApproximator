#pragma once

struct Point {
    double x;
    double y;
};

struct MatrixMxN {
    double** matrix;
    int rowCount;
    int columnCount;
};

void FreeMatrix(MatrixMxN& matrix);

extern "C" {

    struct _declspec(dllexport) EqualitySolution {
        double* variables;
        int length;
    };

    _declspec(dllexport) void FreeEqualitySolution(EqualitySolution& solution);
}