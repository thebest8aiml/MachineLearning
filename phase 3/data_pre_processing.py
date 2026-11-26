import pandas as pd
from sklearn.model_selection import GroupShuffleSplit

def prepare_data():
    df = pd.read_csv("FoodCollectorData.csv")

    y = df["peak_ram_b"]
    X = df.drop(columns=["peak_ram_b"])

    # Options
    # X = X.drop(columns=["collumn_name"]) # Drop collumns
    # df = df.drop_duplicates(subset="collumn_name") # Drop duplicate rows

    splitter = GroupShuffleSplit(test_size=0.2, random_state=0)
    train_idx, test_idx = next(splitter.split(X, y, groups=df["date"]))
    X_train, X_test = X.iloc[train_idx], X.iloc[test_idx]
    y_train, y_test = y.iloc[train_idx], y.iloc[test_idx]

    return X_train, X_test, y_train, y_test

'''
Ideas for improvements:
    - Stratefy the split. This means that the testing and training sets will have roughly the same distribution for the output

'''