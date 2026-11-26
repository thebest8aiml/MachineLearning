from sklearn.tree import DecisionTreeClassifier
from sklearn.metrics import accuracy_score
from data_pre_processing import prepare_data

X_train, X_test, y_train, y_test = prepare_data()

model = DecisionTreeClassifier(max_depth=10)
model.fit(X_train, y_train)

y_pred = model.predict(X_test)
print("Accuracy:", accuracy_score(y_test, y_pred))