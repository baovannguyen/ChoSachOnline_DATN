import React, { useEffect, useState } from "react";
import axios from "axios";

const apiBase = "/api/Author";

export default function Authors() {
    const [authors, setAuthors] = useState([]);
    const [editingAuthor, setEditingAuthor] = useState(null);
    const [form, setForm] = useState({
        AuthorId: "",
        Name: "",
    });

    const fetchAuthors = async () => {
        try {
            const res = await axios.get(apiBase);
            setAuthors(res.data);
        } catch {
            alert("Lỗi lấy tác giả");
        }
    };

    useEffect(() => {
        fetchAuthors();
    }, []);

    const resetForm = () => {
        setForm({ AuthorId: "", Name: "" });
        setEditingAuthor(null);
    };

    const onChange = (e) => {
        const { name, value } = e.target;
        setForm((f) => ({ ...f, [name]: value }));
    };

    const onSubmit = async (e) => {
        e.preventDefault();
        try {
            if (editingAuthor) {
                await axios.put(`${apiBase}/${form.AuthorId}`, form);
                alert("Cập nhật thành công");
            } else {
                await axios.post(apiBase, form);
                alert("Thêm tác giả thành công");
            }
            fetchAuthors();
            resetForm();
        } catch {
            alert("Lỗi khi lưu tác giả");
        }
    };

    const onEdit = (author) => {
        setForm(author);
        setEditingAuthor(author.AuthorId);
    };

    const onDelete = async (id) => {
        if (!window.confirm("Bạn chắc chắn muốn xóa?")) return;
        try {
            await axios.delete(`${apiBase}/${id}`);
            alert("Xóa thành công");
            fetchAuthors();
        } catch {
            alert("Lỗi khi xóa tác giả");
        }
    };

    return (
        <div>
            <h2>Quản lý Tác giả</h2>
            <form onSubmit={onSubmit}>
                <input
                    name="AuthorId"
                    placeholder="ID tác giả"
                    value={form.AuthorId}
                    onChange={onChange}
                    disabled={editingAuthor !== null}
                    required
                />
                <input
                    name="Name"
                    placeholder="Tên tác giả"
                    value={form.Name}
                    onChange={onChange}
                    required
                />
                <button type="submit">{editingAuthor ? "Cập nhật" : "Thêm"}</button>
                {editingAuthor && <button onClick={resetForm}>Hủy</button>}
            </form>

            <table border="1" cellPadding={5} style={{ marginTop: 20 }}>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Tên tác giả</th>
                        <th>Hành động</th>
                    </tr>
                </thead>
                <tbody>
                    {authors.map((a) => (
                        <tr key={a.AuthorId}>
                            <td>{a.AuthorId}</td>
                            <td>{a.Name}</td>
                            <td>
                                <button onClick={() => onEdit(a)}>Sửa</button>{" "}
                                <button onClick={() => onDelete(a.AuthorId)}>Xóa</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}
