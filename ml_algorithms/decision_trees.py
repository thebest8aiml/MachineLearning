from sklearn import tree
from sklearn.datasets import load_iris

from database import Database


def testtree(self, database: Database):
    iris = load_iris()
    X, y = iris.data, iris.target

    clf = tree.DecisionTreeClassifier()

    clf = clf.fit(X, y)
    tree.plot_tree(clf)
    return clf
