import React, { useEffect, useState } from "react";
import axios from "axios";

const apiBase = "/api/Category";

export default function Categories() {
    const [categories, setCategories] = useState([]);
    const [editingCat, setEditingCat] = useState(null);
    const [form, setForm] = useState({
        categoryId: "",
        categoryName: "",
        description: "",
    });

    const fetchCategories = async () => {
        try {
            const res = await axios.get(apiBase);
            console.log("DATA:", res.data); // ✅ Kiểm tra dữ liệu trả về
            setCategories(res.data);
        } catch {
            alert("Lỗi lấy thể loại");
        }
    };

    useEffect(() => {
        fetchCategories();
    }, []);

    const resetForm = () => {
        setForm({ categoryId: "", categoryName: "", description: "" });
        setEditingCat(null);
    };

    const onChange = (e) => {
        const { name, value } = e.target;
        setForm((f) => ({ ...f, [name]: value }));
    };

    const onSubmit = async (e) => {
        e.preventDefault();
        try {
            if (editingCat) {
                await axios.put(`${apiBase}/${form.categoryId}`, form);
                alert("Cập nhật thành công");
            } else {
                const { categoryId, ...createData } = form; // bỏ ID khi tạo
                await axios.post(apiBase, createData);
                alert("Thêm thể loại thành công");
            }
            fetchCategories();
            resetForm();
        } catch {
            alert("Lỗi khi lưu thể loại");
        }
    };

    const onEdit = (cat) => {
        setForm(cat);
        setEditingCat(cat.categoryId);
    };

    const onDelete = async (id) => {
        if (!window.confirm("Bạn chắc chắn muốn xóa?")) return;
        try {
            await axios.delete(`${apiBase}/${id}`);
            alert("Xóa thành công");
            fetchCategories();
        } catch {
            alert("Lỗi khi xóa thể loại");
        }
    };

    return (
        <div>
            <h2>Quản lý Thể loại</h2>
            <form onSubmit={onSubmit}>
                <input
                    name="categoryId"
                    placeholder="ID thể loại"
                    value={form.categoryId}
                    onChange={onChange}
                    disabled={editingCat !== null}
                    required={!editingCat}
                />
                <input
                    name="categoryName"
                    placeholder="Tên thể loại"
                    value={form.categoryName}
                    onChange={onChange}
                    required
                />
                <input
                    name="description"
                    placeholder="Mô tả"
                    value={form.description}
                    onChange={onChange}
                />
                <button type="submit">{editingCat ? "Cập nhật" : "Thêm"}</button>
                {editingCat && <button type="button" onClick={resetForm}>Hủy</button>}
            </form>

            <table border="1" cellPadding={5} style={{ marginTop: 20 }}>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Tên thể loại</th>
                        <th>Mô tả</th>
                        <th>Hành động</th>
                    </tr>
                </thead>
                <tbody>
                    {categories.map((c) => (
                        <tr key={c.categoryId}>
                            <td>{c.categoryId}</td>
                            <td>{c.categoryName}</td>
                            <td>{c.description}</td>
                            <td>
                                <button onClick={() => onEdit(c)}>Sửa</button>{" "}
                                <button onClick={() => onDelete(c.categoryId)}>Xóa</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}
