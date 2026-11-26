import numpy as np
from data_pre_processing import prepare_data
from sklearn.linear_model import LinearRegression, r2_score, mean_absolute_error, mean_squared_error
import matplotlib.pyplot as plt

X_train, X_test, y_train, y_test = prepare_data()

model = LinearRegression()
model.fit(X_train, y_train)

# Measuring the output from our linear regression model
y_pred = model.predict(X_test)
print("R²:", r2_score(y_test, y_pred))
print("Mean Absolute Error:", mean_absolute_error(y_test, y_pred))
print("Mean Squared Error:", mean_squared_error(y_test, y_pred))
print("Root Mean Squared Error:", np.sqrt(mean_squared_error(y_test, y_pred)))

# Creating a matplot graph to visualize the output
plt.figure(figsize=(10, 6))

plt.scatter(
    y_test,
    y_pred,
    alpha=0.6,
    color="blue"
)

# Linear regression line
min_val = min(y_test.min(), y_pred.min())
max_val = max(y_test.max(), y_pred.max())

plt.plot(
    [min_val, max_val],
    [min_val, max_val],
    color="red",
    linestyle="--",
    label="Perfect prediction"
)

plt.xlabel("Actual values")
plt.ylabel("Predicted values")
plt.title("Linear Regression")

plt.legend()
plt.grid(alpha=0.3)
plt.show()