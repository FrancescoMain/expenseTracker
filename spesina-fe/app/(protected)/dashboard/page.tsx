"use client";

import { useEffect, useState } from "react";
import { useAuth } from "@/context/AuthContext";
import { apiFetch } from "@/lib/apiFetch";
import { Progress } from "@/components/ui/progress";
import { Card, CardContent } from "@/components/ui/card";
import { PieChart, Pie, Cell, Tooltip, ResponsiveContainer } from "recharts";

const COLORS = [
  "#0088FE",
  "#00C49F",
  "#FFBB28",
  "#FF8042",
  "#B266FF",
  "#FF6666",
  "#FFB266",
  "#66FF66",
  "#66B2FF",
  "#FF66B2",
  "#FF66FF",
  "#FFB2FF",
  "#B2FF66",
  "#B266FF",
  "#FFB266",
  "#66B2FF",
  "#FF6666",
  "#FFBB28",
  "#00C49F",
  "#0088FE",
  "#FF8042",
  "#B266FF",
  "#FF6666",
];
type Summary = {
  totalIncome: number;
  totalExpenses: number;
  netBalance: number;
  byCategory: Record<string, number>;
};
export default function Dashboard() {
  const { token } = useAuth();
  const [summary, setSummary] = useState<Summary | null>(null);
  const [budgets, setBudgets] = useState<any[]>([]);
  const [subcategories, setSubcategories] = useState<any>(null);

  const [account, setAccount] = useState<any>(null);
  const [goals, setGoals] = useState<any[]>([]);
  const [transactions, setTransactions] = useState<any[]>([]);
  const [selectedMonth, setSelectedMonth] = useState(() =>
    new Date().toISOString().slice(0, 7)
  );

  const fetchData = async () => {
    const headers = { Authorization: `Bearer ${token}` };

    const transactionRes = await apiFetch(
      `http://localhost:5299/api/Transaction?month=${selectedMonth}`,
      { headers }
    );
    const transactionData = await transactionRes.json();
    const tx = transactionData.data;
    setTransactions(tx);

    // 👉 Riassunto dinamico
    const totalIncome = tx
      .filter((t: any) => t.amount > 0)
      .reduce((acc: number, t: any) => acc + t.amount, 0);
    const totalExpenses = tx
      .filter((t: any) => t.amount < 0)
      .reduce((acc: number, t: any) => acc + t.amount, 0);
    const netBalance = totalIncome + totalExpenses;

    // 👉 Totali per categoria
    const byCategory: Record<string, number> = {};
    tx.forEach((t: any) => {
      const key = t.categoryName || "Altro";
      byCategory[key] = (byCategory[key] || 0) + t.amount;
    });

    const bySubcategory = tx
      .filter((t: any) => t.amount < 0)
      .reduce((acc: Record<string, number>, t: any) => {
        const name = t.subcategoryName || "Sconosciuta";
        acc[name] = (acc[name] || 0) + Math.abs(t.amount);
        return acc;
      }, {});

    setSubcategories(bySubcategory);
    setSummary({
      totalIncome,
      totalExpenses: Math.abs(totalExpenses),
      netBalance,
      byCategory,
    });

    const accountRes = await apiFetch("http://localhost:5299/api/Account", {
      headers,
    });
    const budgetRes = await apiFetch("http://localhost:5299/api/Budget", {
      headers,
    });
    const goalRes = await apiFetch("http://localhost:5299/api/SavingGoal", {
      headers,
    });

    setAccount((await accountRes.json()).data);
    setBudgets((await budgetRes.json()).data);
    setGoals((await goalRes.json()).data);
  };

  useEffect(() => {
    if (token) fetchData();
  }, [token]);

  console.log(subcategories);

  const getProgressColor = (percentage: number) => {
    console.log(percentage);
    if (percentage < 50) return "bg-green-600";
    if (percentage < 80) return "bg-yellow-500";
    return "bg-red-600";
  };

  return (
    <div className="p-6 space-y-6 tracker">
      <h1 className="text-3xl font-bold mb-4 col-sec">📊 Dashboard</h1>

      {summary && (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <Card>
            <CardContent className="p-4">
              <h2 className="text-lg font-semibold col-sec">Totale Conti</h2>
              <p className="text-xl">€ {account?.totalBalance?.toFixed(2)}</p>
            </CardContent>
          </Card>
          <Card>
            <CardContent className="p-4">
              <h2 className="text-lg font-semibold col-sec">Entrate</h2>
              <p className="text-xl text-green-500">
                +€ {summary.totalIncome.toFixed(2)}
              </p>
            </CardContent>
          </Card>
          <Card>
            <CardContent className="p-4">
              <h2 className="text-lg font-semibold col-sec">Uscite</h2>
              <p className="text-xl text-red-500">
                -€ {summary.totalExpenses.toFixed(2)}
              </p>
            </CardContent>
          </Card>
        </div>
      )}

      {/* Grafico spese per categoria */}
      {summary && (
        <div className="bc-ter rounded-xl p-6 border shadow">
          <h2 className="text-xl font-semibold mb-4 col-sec">
            📌 Spese per categoria
          </h2>

          {/* Lista delle categorie con colori */}
          <div className="flex flex-wrap gap-4 mb-4">
            {Object.entries(summary.byCategory)
              .filter(([_, v]) => v < 0)
              .map(([name], i) => (
                <div key={i} className="flex items-center gap-2 text-sm">
                  <div
                    className="w-4 h-4 rounded"
                    style={{ backgroundColor: COLORS[i % COLORS.length] }}
                  ></div>
                  <span>{name}</span>
                </div>
              ))}
          </div>

          {/* Grafico */}
          <ResponsiveContainer width="100%" height={500}>
            <PieChart>
              <Pie
                data={Object.entries(summary.byCategory)
                  .filter(([_, value]) => value < 0)
                  .map(([name, value]) => ({
                    name,
                    value: Math.abs(value as number),
                  }))}
                dataKey="value"
                nameKey="name"
                outerRadius={250}
              >
                {Object.entries(summary.byCategory)
                  .filter(([_, value]) => value < 0)
                  .map(([_, __], index) => (
                    <Cell
                      key={`cell-${index}`}
                      fill={COLORS[index % COLORS.length]}
                    />
                  ))}
              </Pie>
              <Tooltip
                formatter={(value: any) => `€ ${parseFloat(value).toFixed(2)}`}
              />
            </PieChart>
          </ResponsiveContainer>
        </div>
      )}
      {transactions.length > 0 && (
        <div className="bc-ter rounded-xl p-6 border shadow">
          <h2 className="text-xl font-semibold mb-4 col-sec">
            🧁 Spese per sottocategoria
          </h2>

          <div className="flex flex-wrap gap-4 mb-4">
            {Object.entries(subcategories).map(([name], i) => (
              <div key={i} className="flex items-center gap-2 text-sm">
                <div
                  className="w-4 h-4 rounded"
                  style={{ backgroundColor: COLORS[i % COLORS.length] }}
                ></div>
                <span>{name}</span>
              </div>
            ))}
          </div>

          <ResponsiveContainer width="100%" height={500}>
            <PieChart>
              <Pie
                data={Object.entries(subcategories).map(([name, value]) => ({
                  name,
                  value,
                }))}
                dataKey="value"
                nameKey="name"
                outerRadius={250}
              >
                {Object.entries(subcategories).map(([_, __], index) => (
                  <Cell
                    key={`cell-sub-${index}`}
                    fill={COLORS[index % COLORS.length]}
                  />
                ))}
              </Pie>
              <Tooltip
                formatter={(value: any) => `€ ${parseFloat(value).toFixed(2)}`}
              />
            </PieChart>
          </ResponsiveContainer>
        </div>
      )}

      {/* Budget */}
      {budgets.length > 0 && (
        <div className="bc-ter rounded-xl p-6 border shadow">
          <h2 className="text-xl font-semibold mb-4 col-sec">
            🎯 Budget Attivi
          </h2>
          <div className="space-y-4">
            {budgets.map((b, idx) => {
              const spent = Math.abs(summary?.byCategory[b.categoryName] || 0);
              const perc = Math.min((spent / b.amount) * 100, 100);
              return (
                <div key={idx}>
                  <div className="flex justify-between mb-1">
                    <span className="font-medium">
                      {b.categoryName} ({b.subcategoryName || "Tutte"})
                    </span>
                    <span>
                      €{spent.toFixed(2)} / €{b.amount.toFixed(2)} (
                      {perc.toFixed(0)}%)
                    </span>
                  </div>
                  <Progress value={perc} className={getProgressColor(perc)} />
                </div>
              );
            })}
          </div>
        </div>
      )}

      {/* Obiettivi di risparmio */}
      {goals.length > 0 && (
        <div className="bc-ter rounded-xl p-6 border shadow">
          <h2 className="text-xl font-semibold mb-4 col-sec">
            💰 Obiettivi di Risparmio
          </h2>
          <div className="space-y-4">
            {goals.map((goal) => {
              const perc = Math.min(
                (goal.currentAmount / goal.targetAmount) * 100,
                100
              );
              return (
                <div key={goal.id}>
                  <div className="flex justify-between mb-1">
                    <span className="font-medium">{goal.name}</span>
                    <span>
                      €{goal.currentAmount.toFixed(2)} / €
                      {goal.targetAmount.toFixed(2)} ({perc.toFixed(0)}%)
                    </span>
                  </div>
                  <Progress value={perc} className={getProgressColor(perc)} />
                </div>
              );
            })}
          </div>
        </div>
      )}
    </div>
  );
}
