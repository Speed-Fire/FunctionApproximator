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

extern "C" {

    struct _declspec(dllexport) Array {
        double* variables;
        size_t length;
    };

    _declspec(dllexport) void FreeArray(Array& solution);
}

void FreeMatrix(MatrixMxN& matrix);